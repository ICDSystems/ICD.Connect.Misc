using Crestron.SimplSharpPro.Keypads;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads
{
	public abstract class AbstractInetCbdexAdapter<TKeypad, TSettings> : AbstractKeypadBaseAdapter<TKeypad, TSettings>
		where TKeypad : InetCbdex
		where TSettings : IInetCbdexAdapterSettings, new()
	{
		 
	}
}