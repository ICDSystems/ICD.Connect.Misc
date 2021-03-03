using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Misc.Windows.Devices.WindowsPeripheralDevice;
using ICD.Connect.Misc.Windows.WindowsPeripheral.Factories;
using Microsoft.Management.Infrastructure;

namespace ICD.Connect.Misc.Windows.WindowsPeripheral.FactoryCollections
{
	public sealed class UsbWindowsPeripheralFactoryCollection : AbstractWindowsPeripheralFactoryCollection<UsbWindowsPeripheralFactory>
	{
		private readonly Dictionary<UsbProductInfo, UsbWindowsPeripheralFactory> m_UsbDevicesWhitelistLookup;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="peripheralComponent"></param>
		public UsbWindowsPeripheralFactoryCollection(WindowsPeripheralComponent peripheralComponent)
			: base(peripheralComponent)
		{
			m_UsbDevicesWhitelistLookup = new Dictionary<UsbProductInfo, UsbWindowsPeripheralFactory>();

			IcdEnvironment.OnSystemDeviceAddedRemoved += IcdEnvironmentOnSystemDeviceAddedRemoved;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			IcdEnvironment.OnSystemDeviceAddedRemoved -= IcdEnvironmentOnSystemDeviceAddedRemoved;
		}

		public override void AddFactoryFromXml(string xml)
		{
			UsbWindowsPeripheralFactory factory = new UsbWindowsPeripheralFactory();
			factory.ParseXml(xml);
			AddFactory(factory);
		}

		public override void AddFactory(UsbWindowsPeripheralFactory factory)
		{
			base.AddFactory(factory);

			m_UsbDevicesWhitelistLookup.AddRange(factory.GetProductInfos(), i => factory);
		}

		private void IcdEnvironmentOnSystemDeviceAddedRemoved(object sender, BoolEventArgs e)
		{
			ThreadingUtils.SafeInvoke(UpdatePeripherals);
		}

		/// <summary>
		/// When called, tells the peripheral factory collection to update its peripherals
		/// Typically called on StartSettings to add connected peripherals from the whitelist
		/// </summary>
		public override void UpdatePeripherals()
		{
			// null instead of localhost which would otherwise require certain MMI services running
			using (CimSession session = CimSession.Create(null))
			{
				IEnumerable<CimInstance> instances = session.QueryInstances(@"root\cimv2", "WQL", @"Select * From Win32_USBHub");
				CheckUsbDeviceWhitelist(instances);
			}
		}

		private void CheckUsbDeviceWhitelist(IEnumerable<CimInstance> usbDevices)
		{
			IEnumerable<IWindowsPeripheralDevice> devices = LazyLoadDevices(usbDevices);
			PeripheralComponent.RegisterPeripherals(devices);
		}

		private IEnumerable<IWindowsPeripheralDevice> LazyLoadDevices(IEnumerable<CimInstance> usbDevices)
		{
			return usbDevices.Select(LazyLoadDevice)
			                 .Where(device => device != null);
		}

		[CanBeNull]
		private IWindowsPeripheralDevice LazyLoadDevice(CimInstance usbDevice)
		{
			string deviceId = usbDevice.CimInstanceProperties["DeviceID"]?.Value?.ToString();
			if (string.IsNullOrEmpty(deviceId))
				return null;

			// Check if device is already added
			IWindowsPeripheralDevice device;
			if (PeripheralComponent.TryGetPeripheral(deviceId, out device))
				return device;

			// Convert to a UsbProductInfo
			UsbProductInfo usbProductInfo;
			try
			{
				usbProductInfo = UsbProductInfoExtensions.FromDeviceId(deviceId);
			}
			// Some devices (root hubs) don't have vendor and product ID's, and throw an exception
			// Just ignore those devices
			catch (ArgumentException)
			{
				return null;
			}

			// Check for a match from the whitelist
			UsbWindowsPeripheralFactory usbWindowsPeripheralFactory;
			if (!m_UsbDevicesWhitelistLookup.TryGetValue(usbProductInfo, out usbWindowsPeripheralFactory))
				return null;

			// Create the device and add it
			return usbWindowsPeripheralFactory.InstantiateDevice(deviceId, PeripheralComponent.ControlSystem);
		}
	}
}
