using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls
{
	public sealed class VibeBoardAppControl : AbstractDeviceControl<VibeBoard>
	{
		public event EventHandler OnAppLaunchFailed;
		public event EventHandler OnAppLaunched;

		private readonly PackageComponent m_PackageComponent;
		private readonly StartComponent m_StartComponent;

		private static readonly Dictionary<eVibeApps, string> m_AppPackageNames = new Dictionary<eVibeApps, string>()
		{
			{eVibeApps.Chrome, "com.android.chrome"},
			{eVibeApps.Youtube, "com.google.android.youtube"},
			{eVibeApps.Slack, "com.Slack"}
		};
		private static readonly Dictionary<eVibeApps, string> m_AppActivityNames = new Dictionary<eVibeApps, string>()
		{
			{eVibeApps.Chrome, "com.google.android.apps.chrome.Main"},
			{eVibeApps.Youtube, ".app.honeycomb.Shell$HomeActivity"},
			{eVibeApps.Slack, ".ui.HomeActivity"}
		};

		public VibeBoardAppControl(VibeBoard parent, int id) : base(parent, id)
		{
			m_PackageComponent = parent.Components.GetComponent<PackageComponent>();
			m_StartComponent = parent.Components.GetComponent<StartComponent>();
		}

		public void LaunchApp(eVibeApps app)
		{
			string packageName = GetPackageName(app);

			// TODO check if app is actually installed
			//if (!IsInstalled(packageName))
			//{
			//	OnAppLaunchFailed.Raise(this);
			//	return;
			//}

			string activityName = GetActivityName(app);
			m_StartComponent.StartActivity(packageName, activityName);
		}

		public string GetPackageName(eVibeApps app)
		{
			if (!m_AppPackageNames.ContainsKey(app))
				throw new InvalidOperationException(string.Format("No package name exists for app {0}", app));

			return m_AppPackageNames[app];
		}

		public string GetActivityName(eVibeApps app)
		{
			if (!m_AppActivityNames.ContainsKey(app))
				throw new InvalidOperationException(string.Format("No activity name exists for app {0}", app));

			return m_AppActivityNames[app];
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
				OnAppLaunched.Raise(this);

			if (response.Error != null)
				Log(eSeverity.Error, response.Error.Message);
		}
	}

	public enum eVibeApps
	{
		Chrome,
		Youtube,
		Slack,
		Whiteboard,
	}
}
