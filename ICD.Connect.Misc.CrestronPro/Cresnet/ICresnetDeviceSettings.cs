using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.Cresnet
{
	public interface ICresnetDeviceSettings : IDeviceSettings
	{
		CresnetDeviceSettings CresnetDeviceSettings { get; }
	}
}