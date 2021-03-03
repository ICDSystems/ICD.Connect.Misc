using System;
using System.Collections.Generic;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Xml;
using ICD.Common.Logging.LoggingContexts;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Misc.Windows.Devices.ControlSystems;
using ICD.Connect.Misc.Windows.Devices.WindowsPeripheralDevice;
using ICD.Connect.Misc.Windows.WindowsPeripheral.FactoryCollections;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Cores;

namespace ICD.Connect.Misc.Windows.WindowsPeripheral
{
	public sealed class WindowsPeripheralComponent : IDisposable
	{
		private const string TYPE_ATTRIBUTE = "type";
		private const string PERIPHERAL_FACTORY_ELEMENT = "PeripheralFactory";

		private readonly WindowsControlSystem m_ControlSystem;
		private readonly Dictionary<string, IWindowsPeripheralFactoryCollection> m_FactoryCollections;
		private readonly Dictionary<string, IWindowsPeripheralDevice> m_PeripheralDevicesById;

		/// <summary>
		/// Contains the map of factory type names to factory types
		/// No factories valid for SIMPLSHARP
		/// </summary>
		private static readonly Dictionary<string, Type> s_FactoryCollectionsMap = new Dictionary<string, Type>
		{
#if !SIMPLSHARP
			{"USB", typeof(UsbWindowsPeripheralFactoryCollection)}
#endif
		};

		public WindowsControlSystem ControlSystem { get { return m_ControlSystem; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="controlSystem"></param>
		public WindowsPeripheralComponent([NotNull] WindowsControlSystem controlSystem)
		{
			if (controlSystem == null)
				throw new ArgumentNullException("controlSystem");

			m_ControlSystem = controlSystem;
			m_FactoryCollections = new Dictionary<string, IWindowsPeripheralFactoryCollection>();
			m_PeripheralDevicesById = new Dictionary<string, IWindowsPeripheralDevice>();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Clear();
		}

		#region Methods

		/// <summary>
		/// Clears all peripheral factories and caches
		/// </summary>
		public void Clear()
		{
			foreach (IWindowsPeripheralFactoryCollection factoryCollection in m_FactoryCollections.Values)
				factoryCollection.Dispose();
			m_FactoryCollections.Clear();

			m_PeripheralDevicesById.Clear();
		}

		/// <summary>
		/// Adds the device to the peripheral collection.
		/// </summary>
		/// <param name="device"></param>
		public void RegisterPeripheral(IWindowsPeripheralDevice device)
		{
			RegisterPeripherals(device.Yield());
		}

		/// <summary>
		/// Adds the devices to the peripheral collection.
		/// </summary>
		/// <param name="devices"></param>
		public void RegisterPeripherals(IEnumerable<IWindowsPeripheralDevice> devices)
		{
			bool change = false;

			foreach (IWindowsPeripheralDevice device in devices)
			{
				if (m_PeripheralDevicesById.ContainsKey(device.DeviceId))
					continue;

				m_PeripheralDevicesById.Add(device.DeviceId, device);
				change = true;
			}

			if (!change)
				return;

			// Save settings with new devices
			ICoreSettings settings = ControlSystem.Core.CopySettings();
			FileOperations.SaveSettings(settings, true);
		}

		/// <summary>
		/// Removes the device from the peripheral collection.
		/// </summary>
		/// <param name="device"></param>
		public void DeregisterPeripheral(IWindowsPeripheralDevice device)
		{
			m_PeripheralDevicesById.Remove(device.DeviceId);
		}

		/// <summary>
		/// Checks to see if a peripheral with the given DeviceID is registered
		/// </summary>
		/// <param name="deviceId"></param>
		/// <returns></returns>
		public bool ContainsPeripheral([NotNull] string deviceId)
		{
			if (deviceId == null)
				throw new ArgumentNullException("deviceId");

			return m_PeripheralDevicesById.ContainsKey(deviceId);
		}

		/// <summary>
		/// Tries to get the peripheral registered to the control system with the given DeviceID
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="device"></param>
		/// <returns></returns>
		public bool TryGetPeripheral(string deviceId, out IWindowsPeripheralDevice device)
		{
			return m_PeripheralDevicesById.TryGetValue(deviceId, out device);
		}

		/// <summary>
		/// When called, tells all the peripheral factory collections to update their peripherals
		/// Typically called on StartSettings to add connected peripherals from the whitelist
		/// </summary>
		public void UpdatePeripherals()
		{
			foreach (IWindowsPeripheralFactoryCollection factoryCollection in m_FactoryCollections.Values)
				factoryCollection.UpdatePeripherals();
		}

		public void LoadPeripheralConfig([NotNull] string path)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentException("Value cannot be null or empty.", "path");

			string fullPath =
				PathUtils.GetDefaultConfigPath(WindowsControlSystemSettings.PERIPHERALS_WHITELIST_FOLDER, path);

			if (!IcdFile.Exists(fullPath))
			{
				ControlSystem.Logger.Log(eSeverity.Error, "Failed to load integration config {0} - Path does not exist",
				                         fullPath);
				return;
			}

			try
			{
				string xml = IcdFile.ReadToEnd(fullPath, new UTF8Encoding(false));
				xml = EncodingUtils.StripUtf8Bom(xml);

				foreach (string peripheralXml in XmlUtils.GetChildElementsAsString(xml, PERIPHERAL_FACTORY_ELEMENT))
				{
					try
					{
						ParsePeripheralFactoryConfig(peripheralXml);
					}
					catch (Exception ex)
					{
						ControlSystem.Logger.Log(eSeverity.Error, ex, "Exception parsing Peripheral config:{0}",
						                         ex.Message);
					}
				}

			}
			catch (Exception e)
			{
				ControlSystem.Logger.Log(eSeverity.Error, e, "Failed to load peripheral config {0} - {1}", fullPath,
				                         e.Message);
			}
		}

		#endregion

		#region Private Methods

		private void ParsePeripheralFactoryConfig([NotNull] string xml)
		{
			if (xml == null)
				throw new ArgumentNullException("xml");

			string type = XmlUtils.GetAttributeAsString(xml, TYPE_ATTRIBUTE);

			IWindowsPeripheralFactoryCollection factoryCollection = GetOrCreateFactoryCollection(type);

			factoryCollection.AddFactoryFromXml(xml);
		}

		private IWindowsPeripheralFactoryCollection GetOrCreateFactoryCollection([NotNull] string type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			IWindowsPeripheralFactoryCollection factoryCollection;
			if (!m_FactoryCollections.TryGetValue(type, out factoryCollection))
			{
				Type factoryCollectionType;
				if (!s_FactoryCollectionsMap.TryGetValue(type, out factoryCollectionType))
					throw new KeyNotFoundException(string.Format("Couldn't find FactoryCollection of Type {0}", type));

				factoryCollection =
					(IWindowsPeripheralFactoryCollection)ReflectionUtils.CreateInstance(factoryCollectionType, this);

				m_FactoryCollections.Add(type, factoryCollection);
			}

			return factoryCollection;
		}

		#endregion
	}
}
