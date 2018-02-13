using System;
using ICD.Connect.Misc.Keypads;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads
{
	public static class ButtonStateConverter
	{
#if SIMPLSHARP
		public static eButtonState GetButtonState(Crestron.SimplSharpPro.DeviceSupport.eButtonState state)
		{
			switch (state)
			{
				case Crestron.SimplSharpPro.DeviceSupport.eButtonState.NA:
					return eButtonState.NA;
				case Crestron.SimplSharpPro.DeviceSupport.eButtonState.Pressed:
					return eButtonState.Pressed;
				case Crestron.SimplSharpPro.DeviceSupport.eButtonState.Released:
					return eButtonState.Released;
				case Crestron.SimplSharpPro.DeviceSupport.eButtonState.Tapped:
					return eButtonState.Tapped;
				case Crestron.SimplSharpPro.DeviceSupport.eButtonState.DoubleTapped:
					return eButtonState.DoubleTapped;
				case Crestron.SimplSharpPro.DeviceSupport.eButtonState.Held:
					return eButtonState.Held;
				case Crestron.SimplSharpPro.DeviceSupport.eButtonState.HeldReleased:
					return eButtonState.HeldReleased;
				case Crestron.SimplSharpPro.DeviceSupport.eButtonState.Backlight:
					return eButtonState.Backlight;
				default:
					throw new ArgumentOutOfRangeException("state");
			}
		}
#endif
	}
}