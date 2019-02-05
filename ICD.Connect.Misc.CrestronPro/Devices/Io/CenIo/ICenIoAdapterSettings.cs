using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
	public interface ICenIoAdapterSettings : IDeviceSettings
	{
		[CrestronByteSettingsProperty]
		byte? Ipid { get; set; }
	}
}