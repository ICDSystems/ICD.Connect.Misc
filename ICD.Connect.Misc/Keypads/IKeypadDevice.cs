using System;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.Keypads
{
	public interface IKeypadDevice : IDevice
	{
		event EventHandler<KeypadButtonPressedEventArgs> OnButtonStateChange;
	}
}