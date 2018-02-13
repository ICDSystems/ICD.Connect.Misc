using ICD.Connect.Misc.CrestronPro.Devices.Keypads.KeypadBase;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.InetCbdex
{
	public interface IInetCbdexAdapterSettings : IKeypadBaseAdapterSettings
	{
		ushort? BargraphTimeout { get; set; }
		ushort? HoldTime { get; set; }
		ushort? DoubleTapSpeed { get; set; }
		bool? WaitForDoubleTap { get; set; }
	}
}