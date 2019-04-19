using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
	[KrangSettings("GlsOirLclCCn", typeof(GlsOirLclCCnAdapter))]
	public sealed class GlsOirLclCCnAdapterSettings : AbstractCresnetOccupancySensorAdapterSettings
	{
	}
}