using System;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads
{
	public interface IKeypadDevice : IDevice
	{
		event EventHandler<KeypadButtonPressedEventArgs> OnButtonStateChange;
	}
}