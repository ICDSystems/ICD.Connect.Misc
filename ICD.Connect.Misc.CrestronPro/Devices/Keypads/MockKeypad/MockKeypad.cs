using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.MockKeypad
{
	public sealed class MockKeypad : AbstractKeypadDevice<MockKeypadSettings>, IMockKeypad
	{
		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return true;
		}

		public override event EventHandler<KeypadButtonPressedEventArgs> OnButtonStateChange;

		private void SimulateButtonFeedback(uint buttonId, eButtonState pressType)
		{
			OnButtonStateChange.Raise(this, new KeypadButtonPressedEventArgs{ButtonId = buttonId, ButtonState = pressType});
		}

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<uint, eButtonState>("SimulateFeedback", "Simulates feedback from this keypad.", (a, b) => SimulateButtonFeedback(a, b));
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}