using System;
using ICD.Common.Properties;
using ICD.Connect.Misc.Windows.Devices.ControlSystems;
using ICD.Connect.Misc.Windows.Devices.WindowsPeripheralDevice;

namespace ICD.Connect.Misc.Windows.WindowsPeripheral.Factories
{
	public interface IWindowsPeripheralFactory
	{
		IWindowsPeripheralDevice InstantiateDevice([NotNull] string deviceId,
		                                           [NotNull] WindowsControlSystem controlSystem);

		/// <summary>
		/// Name for devices created by the factory
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Description for devices created by the factory
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Make for devices created by the factory
		/// </summary>
		string Make { get; }

		/// <summary>
		/// Model for devices created by the factory
		/// </summary>
		string Model { get; }

		/// <summary>
		/// RoomCritical state for devices created by the factory
		/// </summary>
		bool RoomCritical { get; }

		/// <summary>
		/// Returns a guid for the specific peripheral type
		/// Used to prevent possible collisions of seeded guids
		/// </summary>
		Guid PeripheralTypeGuid { get; }
	}

	public interface IWindowsPeripheralFactory<T> : IWindowsPeripheralFactory where T : IWindowsPeripheralDevice, new()
	{
		new T InstantiateDevice([NotNull] string deviceId,
		                        [NotNull] WindowsControlSystem controlSystem);
	}
}
