using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.GlobalCache.Devices.IP2IR
{
	[KrangSettings("GlobalCacheIP2IR", typeof(GcIp2IrDevice))]
	public sealed class GcIp2IrDeviceSettings : AbstractGcITachDeviceSettings
	{
	}
}