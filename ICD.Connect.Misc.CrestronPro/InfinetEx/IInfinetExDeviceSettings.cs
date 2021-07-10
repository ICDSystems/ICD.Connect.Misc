using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.InfinetEx
{
	public interface IInfinetExDeviceSettings : IDeviceSettings
	{
		/// <summary>
		/// Contains InfinetEx Settings Data
		/// </summary>
		InfinetExSettings InfinetExSettings { get; }
	}
}