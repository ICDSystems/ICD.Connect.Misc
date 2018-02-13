using ICD.Connect.Misc.CrestronPro.Devices.Keypads.KeypadBase;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.InetCbdex
{
	public abstract class AbstractInetCbdexAdapter<TKeypad, TSettings> : AbstractKeypadBaseAdapter<TKeypad, TSettings>
		where TKeypad : Crestron.SimplSharpPro.Keypads.InetCbdex
		where TSettings : IInetCbdexAdapterSettings, new()
	{
		 
	}
}