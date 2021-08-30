#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
#if !NETSTANDARD
	public sealed class GlsOirCCnAdapter : AbstractCresnetOccupancySensorAdapter<GlsOirCCnAdapterSettings,GlsOirCCn>
#else
	public sealed class GlsOirCCnAdapter : AbstractCresnetOccupancySensorAdapter<GlsOirCCnAdapterSettings>
#endif
	{
#if !NETSTANDARD
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