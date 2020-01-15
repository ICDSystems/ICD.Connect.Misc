using System;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class StartComponent : AbstractVibeComponent
	{
		public event EventHandler OnAppLaunched;
		public event EventHandler OnAppLaunchFailed;

		private const string COMMAND = "start";
		private const string PARAMS = "-n {0}/{1}";

		public StartComponent(VibeBoard parent) : base(parent)
		{
		}
		
		public void StartActivity(string packageName, string activityName)
		{
			string param = string.Format(PARAMS, packageName, activityName);
			Parent.SendCommand(new VibeCommand(COMMAND, param));
		}

		protected override void Subscribe(VibeBoard parent)
		{
			base.Subscribe(parent);

			parent.ResponseHandler.RegisterResponseCallback<StartActivityResponse>(StartActivityResponseCallback);
		}

		protected override void Unsubscribe(VibeBoard parent)
		{
			base.Unsubscribe(parent);
			
			parent.ResponseHandler.UnregisterResponseCallback<StartActivityResponse>(StartActivityResponseCallback);
		}

		private void StartActivityResponseCallback(StartActivityResponse response)
		{
			if (response.Value != null && response.Value.Success)
			{
				OnAppLaunched.Raise(this);
				return;
			}

			if (response.Error != null)
			{
				Log(eSeverity.Error, response.Error.Message);
				OnAppLaunchFailed.Raise(this);
			}
		}
	}
}
