using ICD.Connect.Devices;

namespace ICD.Connect.Misc.InfinetEx
{
	public interface IInfinetExDevice : IDevice
	{
		InfinetExInfo InfinetExInfo { get; }
	}
}