using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
	public interface ICresnetBridgeAdapterSettings : IDeviceSettings
	{
		byte? Ipid { get; set; }
	}
}