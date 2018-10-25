using ICD.Connect.Devices;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	public interface IGcITachDeviceSettings : IDeviceSettings
	{
		/// <summary>
		/// The port id.
		/// </summary>
		int? Port { get; set; }
	}
}
