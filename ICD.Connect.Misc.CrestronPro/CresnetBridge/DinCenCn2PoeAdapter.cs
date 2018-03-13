﻿#if SIMPLSHARP
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.CresnetBridge
{
#if SIMPLSHARP
	public sealed class DinCenCn2PoeAdapter : AbstractDinCenCn2Adapter<DinCenCn2Poe, DinCenCn2PoeAdapterSettings>
#else
	public sealed class DinCenCn2PoeAdapter : AbstractDinCenCn2Adapter<DinCenCn2PoeAdapterSettings>
#endif
	{
#if SIMPLSHARP
		protected override DinCenCn2Poe InstantiateBridge(byte ipid)
		{
			return new DinCenCn2Poe(ipid, ProgramInfo.ControlSystem);
		}
#endif
	}
}