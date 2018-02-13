using System;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads
{
	public abstract class AbstractKeypadDevice<TSettings> : AbstractDevice<TSettings>, IKeypadDevice
		where TSettings : IKeypadDeviceSettings, new()
	{
		public abstract event EventHandler<KeypadButtonPressedEventArgs> OnButtonStateChange;
	}
}