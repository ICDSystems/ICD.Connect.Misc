using ICD.Connect.Misc.CrestronPro.Devices.Keypads.KeypadBase;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.InetCbdex
{
#if !NETSTANDARD
	public abstract class AbstractInetCbdexAdapter<TKeypad, TSettings> : AbstractKeypadBaseAdapter<TKeypad, TSettings>
		where TKeypad : Crestron.SimplSharpPro.Keypads.InetCbdex
#else
	public abstract class AbstractInetCbdexAdapter<TSettings> : AbstractKeypadBaseAdapter<TSettings>
#endif
		where TSettings : IInetCbdexAdapterSettings, new()
	{
		 
	}
}