using Crestron.SimplSharpPro;
using ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBaseWithVersiport;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdP
{
	public sealed class C2nCbdPAdapter : AbstractC2nCbdPBaseWithVersiportAdapter<Crestron.SimplSharpPro.Keypads.C2nCbdP, C2nCbdPAdapterSettings>, IC2nCbdPAdapter
	{


		protected override Crestron.SimplSharpPro.Keypads.C2nCbdP InstantiateKeypad(byte cresnetId, CrestronControlSystem controlSystem)
		{
			return new Crestron.SimplSharpPro.Keypads.C2nCbdP(cresnetId, controlSystem);
		}
	}
}