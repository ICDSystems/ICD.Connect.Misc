using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.Cresnet
{
	public interface ICresnetDevice : IDevice
	{
		CresnetDeviceInfo CresnetDeviceInfo { get; }
	}
}