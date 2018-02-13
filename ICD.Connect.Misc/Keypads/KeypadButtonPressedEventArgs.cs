using System;

namespace ICD.Connect.Misc.Keypads
{
	public sealed class KeypadButtonPressedEventArgs : EventArgs
	{
		public uint ButtonId { get; set; }
		public eButtonState ButtonState { get; set; }
	}
}