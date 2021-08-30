#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
#if !NETSTANDARD
	public sealed class GlsOdtCCnAdapter : AbstractCresnetOccupancySensorAdapter<GlsOdtCCnAdapterSettings,GlsOdtCCn>
#else
	public sealed class GlsOdtCCnAdapter : AbstractCresnetOccupancySensorAdapter<GlsOdtCCnAdapterSettings>
#endif
	{
#if !NETSTANDARD
		protected override GlsOdtCCn InstantiateControlSystem(byte cresnetId, CrestronControlSystem controlSystem)
		{
			return new GlsOdtCCn(cresnetId, controlSystem);
		}

		protected override GlsOdtCCn InstantiateCresnetBranch(byte cresnetId, CresnetBranch cresnetBranch)
		{
			return new GlsOdtCCn(cresnetId, cresnetBranch);
		}
#endif
	}
}