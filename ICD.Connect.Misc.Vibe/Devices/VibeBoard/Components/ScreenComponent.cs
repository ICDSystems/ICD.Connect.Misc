using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices.Controls.Power;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class ScreenComponent : AbstractVibeComponent
	{
		public event EventHandler<PowerStateEventArgs> OnScreenStateChanged;

		private const string COMMAND = "screen";
		private const string PARAM_GET_SCREEN = "-l";
		private const string PARAM_SCREEN_ON = "on";
		private const string PARAM_SCREEN_OFF = "off";
		
		private ePowerState m_ScreenState;

		#region Properties

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

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		public ScreenComponent(VibeBoard parent)
			: base(parent)
		{
			Subscribe(parent);
		}

		/// <summary>
		/// Called to initialize the component.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			GetScreenState();
		}

		#region Methods

		public void GetScreenState()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_GET_SCREEN));
		}

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

			if (vibe == null)
				return;
			
			vibe.ResponseHandler.RegisterResponseCallback<ScreenResponse>(ScreenResponseCallback);
		}

		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);

			if (vibe == null)
				return;
			
			vibe.ResponseHandler.UnregisterResponseCallback<ScreenResponse>(ScreenResponseCallback);
		}

		private void ScreenResponseCallback(ScreenResponse response)
		{
			if (response.Error != null)
			{
				Parent.Logger.Log(eSeverity.Error, "Failed to get/set screen state - {0}", response.Error.Message);
				return;
			}

			ScreenState = response.Value.State ? ePowerState.PowerOn : ePowerState.PowerOff;
			Parent.Logger.Log(eSeverity.Informational, "Screen state updated: {0}", response.Value.State ? "On" : "Off");
		}

		#endregion

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Screen State", ScreenState);
		}

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (var command in base.GetConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("GetScreen", "Gets the current screen state", () => GetScreenState());
			yield return new ConsoleCommand("ScreenOn", "Turns the screen on", () => ScreenOn());
			yield return new ConsoleCommand("ScreenOff", "Turns the screen off", () => ScreenOff());
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
