using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.Windows.Devices.ControlSystems;
using ICD.Connect.Misc.Windows.Devices.WindowsPeripheralDevice;
using ICD.Connect.Settings.Utils;

namespace ICD.Connect.Misc.Windows.WindowsPeripheral.Factories
{
	public abstract class AbstractWindowsPeripheralFactory<T> : IWindowsPeripheralFactory<T> where T:IWindowsPeripheralDevice, new()
	{
		private const string NAME_ELEMENT = "Name";
		private const string DESCRIPTION_ELEMENT = "Description";
		private const string MAKE_ELEMENT = "Make";
		private const string MODEL_ELEMENT = "Model";
		private const string ROOM_CRITICAL_ELEMENT = "RoomCritical";

		/// <summary>
		/// Returns a guid for the specific peripheral type
		/// Used to prevent possible collisions of seeded guids
		/// </summary>
		public abstract Guid PeripheralTypeGuid { get; }

		public string Name { get; private set; }
		public string Description { get; private set; }
		public string Make { get; private set; }
		public string Model { get; private set; }
		public bool RoomCritical { get; private set; }

		/// <summary>
		/// Parse XML and update the factory properties
		/// </summary>
		/// <param name="xml"></param>
		public virtual void ParseXml(string xml)
		{
			Name = XmlUtils.TryReadChildElementContentAsString(xml, NAME_ELEMENT);
			Description = XmlUtils.TryReadChildElementContentAsString(xml, DESCRIPTION_ELEMENT);
			Make = XmlUtils.TryReadChildElementContentAsString(xml, MAKE_ELEMENT);
			Model = XmlUtils.TryReadChildElementContentAsString(xml, MODEL_ELEMENT);
			RoomCritical = XmlUtils.TryReadChildElementContentAsBoolean(xml, ROOM_CRITICAL_ELEMENT) ?? false;
		}

		/// <summary>
		/// Instantiate a new device from this factory, with the given DeviceId for the given WindowsControlSystem
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="controlSystem"></param>
		/// <returns></returns>
		public T InstantiateDevice(string deviceId, WindowsControlSystem controlSystem)
		{
			if (deviceId == null)
				throw new ArgumentNullException("deviceId");

			if (controlSystem == null)
				throw new ArgumentNullException("controlSystem");

			int id = IdUtils.GetNewId(controlSystem.Core.Originators.GetChildrenIds(), eSubsystem.Devices);

			Guid uuid = GuidUtils.Combine(GuidUtils.GenerateSeeded(deviceId.GetStableHashCode()), PeripheralTypeGuid);

			T device = new T
			{
				Name = Name ?? deviceId,
				Description = Description,
				DeviceId = deviceId,
				RoomCritical = RoomCritical,
				Serialize = true,
				Id = id,
				Uuid = uuid,
				ControlSystem = controlSystem
			};

			device.ConfiguredDeviceInfo.Make = Make;
			device.ConfiguredDeviceInfo.Model = Model;

			InstantiateDeviceFinal(device);

			controlSystem.Core.Originators.AddChild(device);

			return device;
		}

		/// <summary>
		/// Override for any factory-specific instantiation that needs to happen.
		/// </summary>
		/// <param name="device"></param>
		protected virtual void InstantiateDeviceFinal(T device)
		{
		}

		IWindowsPeripheralDevice IWindowsPeripheralFactory.InstantiateDevice(string deviceId, WindowsControlSystem controlSystem)
		{
			return InstantiateDevice(deviceId, controlSystem);
		}
	}
}
