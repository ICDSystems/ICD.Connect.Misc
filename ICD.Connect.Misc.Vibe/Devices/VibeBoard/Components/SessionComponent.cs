using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class SessionComponent : AbstractVibeComponent
	{
		public event EventHandler OnSessionEnded;

		private const string COMMAND = "session";
		private const string PARAM_END = "end";

		public SessionComponent(VibeBoard parent) : base(parent)
		{
			Subscribe(parent);
		}

		protected override void Dispose(bool disposing)
		{
			Unsubscribe(Parent);

			base.Dispose(disposing);
		}

		#region API Methods

		public void EndSession()
		{
			Log(eSeverity.Debug, "Ending session");
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_END));
		}

		#endregion

		#region Parent Callbacks

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.RegisterResponseCallback<SessionResponse>(SessionCallback);
		}

		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.UnregisterResponseCallback<SessionResponse>(SessionCallback);
		}

		private void SessionCallback(SessionResponse response)
		{
			if (response.Error != null)
			{
				Log(eSeverity.Error, "Failed to end session - {0}", response.Error.Message);
				return;
			}

			Log(eSeverity.Informational, "Session ended");
			OnSessionEnded.Raise(this);
		}

		#endregion

		#region Console

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (var command in base.GetConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("EndSession", "Ends the board's session", () => EndSession());
		}

		#endregion
	}
}
