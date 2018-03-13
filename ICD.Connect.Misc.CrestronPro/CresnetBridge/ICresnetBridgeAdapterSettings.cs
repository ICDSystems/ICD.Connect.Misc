using ICD.Connect.Devices;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.CrestronPro.CresnetBridge
{
	public interface ICresnetBridgeAdapterSettings : IDeviceSettings
	{
		byte? Ipid { get; set; }
	}
}