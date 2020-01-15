﻿using System;
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
		private ePowerState m_ScreenState;

		public ePowerState ScreenState
		{
			get { return m_ScreenState; }
			private set
			{
				if (value == m_ScreenState)
					return;
				
				m_ScreenState = value;

				OnScreenStateChanged.Raise(this, new PowerStateEventArgs(m_ScreenState));
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		public ScreenComponent(VibeBoard parent)
			: base(parent)
		{
		}

		#region Methods

		public void SetScreenState(ePowerState state)
		{
			string param;

			switch (state)
			{
				case ePowerState.PowerOff:
					param = PARAM_SCREEN_OFF;
					break;
				case ePowerState.PowerOn:
					param = PARAM_SCREEN_ON;
					break;
				default:
					throw new ArgumentOutOfRangeException("state");
			}

			m_RequestedState = state;
			Parent.SendCommand(new VibeCommand(COMMAND, param));
		}

		public void ScreenOn()
		{
			SetScreenState(ePowerState.PowerOn);
		}

		public void ScreenOff()
		{
			SetScreenState(ePowerState.PowerOff);
		}

		#endregion

		#region Parent Callbacks

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
				ScreenState = m_RequestedState;
		}

		#endregion
	}

	public sealed class PowerStateEventArgs : GenericEventArgs<ePowerState>
	{
		public PowerStateEventArgs(ePowerState data)
			: base(data)
		{
		}
	}
}