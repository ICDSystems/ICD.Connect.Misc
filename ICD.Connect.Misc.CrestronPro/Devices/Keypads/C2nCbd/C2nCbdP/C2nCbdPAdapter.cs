﻿#if !NETSTANDARD
using Crestron.SimplSharpPro.DeviceSupport;
#endif
using ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBaseWithVersiport;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdP
{
#if !NETSTANDARD
	public sealed class C2nCbdPAdapter : AbstractC2nCbdPBaseWithVersiportAdapter<Crestron.SimplSharpPro.Keypads.C2nCbdP, C2nCbdPAdapterSettings>, IC2nCbdPAdapter
#else
	public sealed class C2nCbdPAdapter : AbstractC2nCbdPBaseWithVersiportAdapter<C2nCbdPAdapterSettings>, IC2nCbdPAdapter
#endif
	{
#if !NETSTANDARD
		protected override Crestron.SimplSharpPro.Keypads.C2nCbdP InstantiateKeypad(byte cresnetId)
		{
			return new Crestron.SimplSharpPro.Keypads.C2nCbdP(cresnetId, ProgramInfo.ControlSystem);
		}

		protected override Crestron.SimplSharpPro.Keypads.C2nCbdP InstantiateKeypad(byte cresnetId, CresnetBranch branch)
		{
			return new Crestron.SimplSharpPro.Keypads.C2nCbdP(cresnetId, branch);
		}
#endif
	}
}