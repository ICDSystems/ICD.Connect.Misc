using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
	[KrangSettings("GlsOdtCCn", typeof(GlsOdtCCnAdapter))]
	public sealed class GlsOdtCCnAdapterSettings : AbstractCresnetOccupancySensorAdapterSettings
	{
	}
}