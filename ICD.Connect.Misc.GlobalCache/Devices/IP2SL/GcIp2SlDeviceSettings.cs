using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.GlobalCache.Devices.IP2SL
{
	[KrangSettings("GlobalCacheIP2SL", typeof(GcIp2SlDeviceSettings))]
	public sealed class GcIp2SlDeviceSettings : AbstractGcITachDeviceSettings
	{
	}
}
