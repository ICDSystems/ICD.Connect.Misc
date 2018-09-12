using System;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.Keypads
{
	public abstract class AbstractKeypadDevice<TSettings> : AbstractDevice<TSettings>, IKeypadDevice
		where TSettings : IKeypadDeviceSettings, new()
	{
		/// <summary>
		/// Raised when a button state changes.
		/// </summary>
		public abstract event EventHandler<KeypadButtonPressedEventArgs> OnButtonStateChange;
	}
}