#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
#if SIMPLSHARP
	public sealed class GlsOirCCnAdapter : AbstractCresnetOccupancySensorAdapter<GlsOirCCnAdapterSettings,GlsOirCCn>
#else
	public sealed class GlsOirCCnAdapter : AbstractCresnetOccupancySensorAdapter<GlsOirCCnAdapterSettings>
#endif
	{
#if SIMPLSHARP
		protected override GlsOirCCn InstantiateControlSystem(byte cresnetId, CrestronControlSystem controlSystem)
		{
			return new GlsOirCCn(cresnetId, controlSystem);
		}

		protected override GlsOirCCn InstantiateCresnetBranch(byte cresnetId, CresnetBranch cresnetBranch)
		{
			return new GlsOirCCn(cresnetId, cresnetBranch);
		}
#endif
	}
}