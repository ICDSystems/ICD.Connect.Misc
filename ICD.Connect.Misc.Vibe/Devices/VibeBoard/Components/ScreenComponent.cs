using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class ScreenComponent : AbstractVibeComponent
	{
		public event EventHandler<PowerStateEventArgs> OnScreenStateChanged;

		private const string COMMAND = "screen";
		private const string PARAM_SCREEN_ON = "on";
		private const string PARAM_SCREEN_OFF = "off";

		private ePowerState m_RequestedState;

		public ScreenComponent(VibeBoard parent) : base(parent)
		{
		}

		public void SetScreenState(ePowerState state)
		{
			m_RequestedState = state;
			Parent.SendCommand(new VibeCommand(COMMAND, state == ePowerState.PowerOn ? PARAM_SCREEN_ON : PARAM_SCREEN_OFF));
		}

		public void ScreenOn()
		{
			SetScreenState(ePowerState.PowerOn);
		}

		public void ScreenOff()
		{
			SetScreenState(ePowerState.PowerOff);
		}

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);
			
			vibe.ResponseHandler.RegisterResponseCallback<ScreenResponse>(ScreenResponseCallback);
		}

		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);
			
			vibe.ResponseHandler.UnregisterResponseCallback<ScreenResponse>(ScreenResponseCallback);
		}

		private void ScreenResponseCallback(ScreenResponse response)
		{
			if (response.Value.Success)
				OnScreenStateChanged.Raise(this, new PowerStateEventArgs(m_RequestedState));
		}
	}

	public class PowerStateEventArgs : GenericEventArgs<ePowerState>
	{
		public PowerStateEventArgs(ePowerState data) : base(data)
		{
		}
	}
}
