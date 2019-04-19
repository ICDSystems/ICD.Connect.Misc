using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
	[KrangSettings("GlsOirCCn", typeof(GlsOirCCnAdapter))]
	public sealed class GlsOirCCnAdapterSettings : AbstractCresnetOccupancySensorAdapterSettings
	{
	}
}