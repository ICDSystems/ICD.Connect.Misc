using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.Devices.CardFrames
{
	public interface ICardFrameDeviceSettings : IDeviceSettings
	{
		byte? Ipid { get; set; }
	}
}
