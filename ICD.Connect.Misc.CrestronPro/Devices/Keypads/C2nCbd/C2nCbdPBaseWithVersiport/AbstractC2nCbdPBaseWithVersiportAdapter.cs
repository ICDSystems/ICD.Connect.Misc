using ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBase;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBaseWithVersiport
{
#if SIMPLSHARP
	public abstract class AbstractC2nCbdPBaseWithVersiportAdapter<TKeypad, TSettings> : AbstractC2nCbdPBaseAdapter<TKeypad, TSettings>, IC2nCbdPBaseWithVersiportAdapter
		where TKeypad : Crestron.SimplSharpPro.Keypads.C2nCbdPBaseWithVersiport
#else
	public abstract class AbstractC2nCbdPBaseWithVersiportAdapter<TSettings> : AbstractC2nCbdPBaseAdapter<TSettings>, IC2nCbdPBaseWithVersiportAdapter
#endif
		where TSettings: IC2nCbdPBaseWithVersiportAdapterSettings, new()
	{
		 
	}
}