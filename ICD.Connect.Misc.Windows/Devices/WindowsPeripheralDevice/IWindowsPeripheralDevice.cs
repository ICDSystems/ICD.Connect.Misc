using ICD.Connect.Devices;
using ICD.Connect.Misc.Windows.Devices.ControlSystems;

namespace ICD.Connect.Misc.Windows.Devices.WindowsPeripheralDevice
{
	public interface IWindowsPeripheralDevice : IDevice
	{
		/// <summary>
		/// Unique identifier for the peripheral device
		/// </summary>
		string DeviceId { get; set; }

		/// <summary>
		/// The windows control system this device is attached to
		/// </summary>
		WindowsControlSystem ControlSystem { get; set; }
	}
}
