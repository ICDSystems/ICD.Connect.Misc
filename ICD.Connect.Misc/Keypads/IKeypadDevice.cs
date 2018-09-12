using System;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.Keypads
{
	public interface IKeypadDevice : IDevice
	{
		/// <summary>
		/// Raised when a button state changes.
		/// </summary>
		event EventHandler<KeypadButtonPressedEventArgs> OnButtonStateChange;
	}
}