using System;
using ICD.Connect.Misc.CrestronPro.Utils;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
#endif
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Misc.Keypads;
using eButtonState = ICD.Connect.Misc.Keypads.eButtonState;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.KeypadBase
{
#if SIMPLSHARP
	public abstract class AbstractKeypadBaseAdapter<TKeypad, TSettings> : AbstractKeypadDevice<TSettings>, IKeypadBaseAdapter
		where TKeypad : Crestron.SimplSharpPro.DeviceSupport.KeypadBase
#else
	public abstract class AbstractKeypadBaseAdapter<TSettings> : AbstractKeypadDevice<TSettings>, IKeypadBaseAdapter
#endif
		where TSettings : IKeypadDeviceSettings, new()
	{
		public override event EventHandler<KeypadButtonPressedEventArgs> OnButtonStateChange;

#if SIMPLSHARP
		protected TKeypad Keypad { get; private set; }
#endif

#if SIMPLSHARP

		/// <summary>
		/// Sets the wrapped device.
		/// </summary>
		/// <param name="device"></param>
		protected void SetKeypad(TKeypad device)
		{
			if (device == Keypad)
				return;

			Unsubscribe(Keypad);

			if (Keypad != null)
				GenericBaseUtils.TearDown(Keypad);

			Keypad = device;

			eDeviceRegistrationUnRegistrationResponse result;
			if (Keypad != null && !GenericBaseUtils.SetUp(Keypad, this, out result))
				Logger.Log(eSeverity.Error, "Unable to register {0} - {1}", Keypad.GetType().Name, result);

			Subscribe(Keypad);
			UpdateCachedOnlineStatus();
		}
#endif

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if SIMPLSHARP
			return Keypad != null && Keypad.IsOnline;
#else
            return false;
#endif
		}

#if SIMPLSHARP
		/// <summary>
		/// Subscribe to the device events.
		/// </summary>
		/// <param name="keypad"></param>
		private void Subscribe(TKeypad keypad)
		{
			if (keypad == null)
				return;

			keypad.OnlineStatusChange += KeypadOnLineStatusChange;
			keypad.ButtonStateChange += KeypadOnButtonStateChange;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="keypad"></param>
		private void Unsubscribe(TKeypad keypad)
		{
			if (keypad == null)
				return;

			keypad.OnlineStatusChange -= KeypadOnLineStatusChange;
			keypad.ButtonStateChange -= KeypadOnButtonStateChange;
		}

		private void KeypadOnLineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		private void KeypadOnButtonStateChange(GenericBase device, ButtonEventArgs args)
		{
			uint button = args.Button.Number;
			eButtonState state = ButtonStateConverter.GetButtonState(args.NewButtonState);

			OnButtonStateChange.Raise(this, new KeypadButtonPressedEventArgs(button, state));
		}
#endif
	}
}