using ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdBase;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBase
{
	public abstract class AbstractC2nCbdPBaseAdapter<TKeypad, TSettings> : AbstractC2nCbdBaseAdapter<TKeypad, TSettings>
		where TKeypad : Crestron.SimplSharpPro.Keypads.C2nCbdPBase
		where TSettings : IC2nCbdPBaseAdapterSettings, new()
	{
		 
	}
}