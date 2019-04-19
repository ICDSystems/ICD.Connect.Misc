#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
#if SIMPLSHARP
	public sealed class GlsOirLclCCnAdapter : AbstractCresnetOccupancySensorAdapter<GlsOirLclCCnAdapterSettings, GlsOirLclCCn>
#else
	public sealed class GlsOirLclCCnAdapter : AbstractCresnetOccupancySensorAdapter<GlsOirLclCCnAdapterSettings>
#endif
	{
#if SIMPLSHARP
		protected override GlsOirLclCCn InstantiateControlSystem(byte cresnetId, CrestronControlSystem controlSystem)
		{
			return new GlsOirLclCCn(cresnetId, controlSystem);
		}

		protected override GlsOirLclCCn InstantiateCresnetBranch(byte cresnetId, CresnetBranch cresnetBranch)
		{
			return new GlsOirLclCCn(cresnetId,cresnetBranch);
		}
#endif
	}
}