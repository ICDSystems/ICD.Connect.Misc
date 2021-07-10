using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.InfinetEx
{
	public interface IInfinetExDevice : IDevice
	{
		InfinetExInfo InfinetExInfo { get; }
	}
}