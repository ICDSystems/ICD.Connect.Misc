using ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdBase;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBase
{
#if SIMPLSHARP
	public abstract class AbstractC2nCbdPBaseAdapter<TKeypad, TSettings> : AbstractC2nCbdBaseAdapter<TKeypad, TSettings>
		where TKeypad : Crestron.SimplSharpPro.Keypads.C2nCbdPBase
#else
	public abstract class AbstractC2nCbdPBaseAdapter<TSettings> : AbstractC2nCbdBaseAdapter<TSettings>
#endif
		where TSettings : IC2nCbdPBaseAdapterSettings, new()
	{
		 
	}
}