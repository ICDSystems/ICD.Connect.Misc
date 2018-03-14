#if SIMPLSHARP
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
#if SIMPLSHARP
	public sealed class DinCenCn2Adapter : AbstractDinCenCn2Adapter<DinCenCn2, DinCenCn2AdapterSettings>
#else
	public sealed class DinCenCn2Adapter : AbstractDinCenCn2Adapter<DinCenCn2AdapterSettings>
#endif
	{
#if SIMPLSHARP
		protected override DinCenCn2 InstantiateBridge(byte ipid)
		{
			return new DinCenCn2(ipid, ProgramInfo.ControlSystem);
		}
#endif
	}
}