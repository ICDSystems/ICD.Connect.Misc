using System;
using System.Collections.Generic;
using ICD.Common.Properties;
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

		/// <summary>
		/// When called, tells the peripheral factory collection to update its peripherals
		/// Typically called on StartSettings to add connected peripherals from the whitelist
		/// </summary>
		public override bool UpdatePeripherals()
		{
			bool output = false;

			// null instead of localhost which would otherwise require certain MMI services running
			using (CimSession session = CimSession.Create(null))
			{
				IEnumerable<CimInstance> instances = session.QueryInstances(@"root\cimv2", "WQL", @"Select * From Win32_USBHub");

				foreach (CimInstance instance in instances)
					output |= InstantiateDiscoveredDevice(instance);
			}

			return output;
		}

		[CanBeNull]
		private bool InstantiateDiscoveredDevice(CimInstance usbDevice)
		{
			string deviceId = usbDevice.CimInstanceProperties["DeviceID"]?.Value?.ToString();
			if (string.IsNullOrEmpty(deviceId))
				return false;

			// Check if device is already added
			IWindowsPeripheralDevice device;
			if (PeripheralComponent.TryGetPeripheral(deviceId, out device))
				return false;

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
				return false;
			}

			// Check for a match from the whitelist
			UsbWindowsPeripheralFactory usbWindowsPeripheralFactory;
			if (!m_UsbDevicesWhitelistLookup.TryGetValue(usbProductInfo, out usbWindowsPeripheralFactory))
				return false;

			// Create the device and add it
			usbWindowsPeripheralFactory.InstantiateDevice(deviceId, PeripheralComponent.ControlSystem);
			return true;
		}
	}
}
