using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class MuteComponent : AbstractVibeComponent
	{
		public event EventHandler<MuteChangedEventArgs> OnMuteChanged;

		private const string COMMAND = "mute";
		private const string PARAM_GET_MUTE = "-l";
		private const string PARAM_MUTE_ON = "on";
		private const string PARAM_MUTE_OFF = "off";

		private bool m_Mute;

		#region Properties

		public bool Mute
		{
			get { return m_Mute; }
			private set
			{
				if (m_Mute == value)
					return;

				m_Mute = value;
				OnMuteChanged.Raise(this, new MuteChangedEventArgs(value));
			}
		}

		#endregion

		public MuteComponent(VibeBoard parent)
			: base(parent)
		{
			Subscribe(parent);
		}

		protected override void Dispose(bool disposing)
		{
			Unsubscribe(Parent);

			base.Dispose(disposing);
		}

		#region API Methods

		public void GetCurrentMute()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_GET_MUTE));
		}

		public void SetMute(bool mute)
		{
			Parent.SendCommand(new VibeCommand(COMMAND, mute ? PARAM_MUTE_ON : PARAM_MUTE_OFF));
		}

		public void MuteOn()
		{
			SetMute(true);
		}

		public void MuteOff()
		{
			SetMute(false);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called to initialize the component.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			GetCurrentMute();
		}

		#endregion

		#region Parent Callbacks

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.RegisterResponseCallback<MuteResponse>(MuteCallback);
		}

		/// <summary>
		/// Unsubscribes from the vibe events.
		/// </summary>
		/// <param name="vibe"></param>
		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.UnregisterResponseCallback<MuteResponse>(MuteCallback);
		}

		private void MuteCallback(MuteResponse response)
		{
			if (response.Error != null)
			{
				Parent.Logger.Log(eSeverity.Error, "Failed to get/set mute - {0}", response.Error.Message);
				return;
			}

			Mute = response.Value.IsMute;
		}

		#endregion

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Mute", Mute);
		}

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (var command in base.GetConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("GetMute", "Gets the current mute state", () => GetCurrentMute());
			yield return new ConsoleCommand("MuteOn", "Sets mute on", () => MuteOn());
			yield return new ConsoleCommand("MuteOff", "Sets mute off", () => MuteOff());
			yield return new GenericConsoleCommand<bool>("SetMute", "SetMute <true/false>", (m) => SetMute(m));
		}

		#endregion
	}

	public sealed class MuteChangedEventArgs : GenericEventArgs<bool>
	{
		public MuteChangedEventArgs(bool data) : base(data)
		{
		}
	}
}
