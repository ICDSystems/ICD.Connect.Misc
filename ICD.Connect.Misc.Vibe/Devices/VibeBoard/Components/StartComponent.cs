using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class StartComponent : AbstractVibeComponent
	{
		public event EventHandler OnAppLaunched;
		public event EventHandler OnAppLaunchFailed;

		private const string COMMAND = "start";
		private const string PARAM_START_ACTIVITY = "-n {0}/{1}";
		private const string PARAM_SCREEN_SWITCHER = "ss";

		public StartComponent(VibeBoard parent) : base(parent)
		{
			Subscribe(parent);
		}

		protected override void Dispose(bool disposing)
		{
			Unsubscribe(Parent);

			base.Dispose(disposing);
		}

		#region API Methods
		
		public void StartActivity(string packageName, string activityName)
		{
			string param = string.Format(PARAM_START_ACTIVITY, packageName, activityName);
			Parent.SendCommand(new VibeCommand(COMMAND, param));
		}

		public void StartScreenSwitcher()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_SCREEN_SWITCHER));
		}

		#endregion

		#region Parent Callbacks

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.RegisterResponseCallback<StartActivityResponse>(StartActivityResponseCallback);
		}

		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);

			if (vibe == null)
				return;
			
			vibe.ResponseHandler.UnregisterResponseCallback<StartActivityResponse>(StartActivityResponseCallback);
		}

		private void StartActivityResponseCallback(StartActivityResponse response)
		{
			// if launch "failed" cause app is already launched, don't count it as fail. It should have switched to it, so still counts
			if (response.Error != null && !response.Error.Message.Equals("Start activity failed. result: 2"))
			{
				Parent.Logger.Log(eSeverity.Error, response.Error.Message);
				OnAppLaunchFailed.Raise(this);
				return;
			}

			Parent.Logger.Log(eSeverity.Informational, "App successfully launched");
			OnAppLaunched.Raise(this);
		}

		#endregion

		#region Console

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (var command in base.GetConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<string, string>("StartActivity",
				"StartActivity <PackageName> <ActivityName>",
				(p, a) => StartActivity(p, a));

			yield return new ConsoleCommand("StartScreenSwitcher", "Launches the screen switcher",
				() => StartScreenSwitcher());
		}

		#endregion
	}
}
