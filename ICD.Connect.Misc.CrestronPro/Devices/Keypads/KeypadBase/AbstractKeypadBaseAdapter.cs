using System;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads
{
	public abstract class AbstractKeypadBaseAdapter<TKeypad, TSettings> : AbstractKeypadDevice<TSettings>, IKeypadBaseAdapter
		where TSettings : IKeypadDeviceSettings, new()
		where TKeypad : Crestron.SimplSharpPro.DeviceSupport.KeypadBase
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
			{
				if (Keypad.Registered)
					Keypad.UnRegister();

				try
				{
					Keypad.Dispose();
				}
				catch
				{
				}
			}

			Keypad = device;

			if (Keypad != null && !Keypad.Registered)
			{
				if (Name != null)
					Keypad.Description = Name;
				eDeviceRegistrationUnRegistrationResponse result = Keypad.Register();
				if (result != eDeviceRegistrationUnRegistrationResponse.Success)
					Logger.AddEntry(eSeverity.Error, "Unable to register {0} - {1}", Keypad.GetType().Name, result);
			}

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
			OnButtonStateChange.Raise(this, new KeypadButtonPressedEventArgs
			{
				ButtonId = args.Button.Number,
				ButtonState = ButtonStateConverter.GetButtonState(args.NewButtonState)
			});
		}
#endif
	}
}