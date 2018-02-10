using ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbdPBase;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBaseWithVersiport
{
	public abstract class AbstractC2nCbdPBaseWithVersiportAdapter<TKeypad, TSettings> : AbstractC2nCbdPBaseAdapter<TKeypad, TSettings>, IC2nCbdPBaseWithVersiportAdapter
		where TKeypad : Crestron.SimplSharpPro.Keypads.C2nCbdPBaseWithVersiport
		where TSettings: IC2nCbdPBaseWithVersiportAdapterSettings, new()
	{
		 
	}
}