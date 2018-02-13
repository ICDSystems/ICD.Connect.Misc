using System;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads
{
	public class KeypadButtonPressedEventArgs : EventArgs
	{
		public uint ButtonId { get; set; }
		public eButtonState ButtonState { get; set; }
	}
}