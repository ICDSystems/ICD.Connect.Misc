using ICD.Connect.Devices;

namespace ICD.Connect.Misc.Windows.Devices.WindowsPeripheralDevice
{
	public interface IWindowsPeripheralDeviceSettings : IDeviceSettings
	{
		/// <summary>
		/// Unique identifier for the peripheral device
		/// </summary>
		string DeviceId { get; set; }

		/// <summary>
		/// The windows control system this device is attached to
		/// </summary>
		int ControlSystem { get; set; }
	}
}
