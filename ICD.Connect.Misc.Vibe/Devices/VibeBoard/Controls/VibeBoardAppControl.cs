using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls
{
	public sealed class VibeBoardAppControl : AbstractDeviceControl<VibeBoard>
	{
		public event EventHandler OnAppLaunchFailed;
		public event EventHandler OnAppLaunched;

		private static readonly Dictionary<eVibeApp, string> s_AppPackageNames = new Dictionary<eVibeApp, string>()
		{
			{eVibeApp.TouchCue, "com.profound.touchcue"},
			{eVibeApp.Chrome, "com.android.chrome"},
			{eVibeApp.Youtube, "com.google.android.youtube"},
			{eVibeApp.Slack, "com.Slack"},
			{eVibeApp.Whiteboard, "ai.inlight.board.app"},
			{eVibeApp.Teams, "com.microsoft.teams"},
			{eVibeApp.WebEx, "com.cisco.webex.meetings"}
		};
		private static readonly Dictionary<eVibeApp, string> s_AppActivityNames = new Dictionary<eVibeApp, string>()
		{
			{eVibeApp.TouchCue, ".ui.MainActivity" },
			{eVibeApp.Chrome, "com.google.android.apps.chrome.Main"},
			{eVibeApp.Youtube, ".app.honeycomb.Shell$HomeActivity"},
			{eVibeApp.Slack, ".ui.HomeActivity"},
			{eVibeApp.Whiteboard, ".MainActivity" },
			{eVibeApp.Teams, "com.microsoft.skype.teams.views.activities.SplashActivity"},
			{eVibeApp.WebEx, ".ui.premeeting.welcome.WebExMeeting"}
		};
		
		private readonly PackageComponent m_PackageComponent;
		private readonly StartComponent m_StartComponent;
		private readonly KeyComponent m_KeyComponent;
		private readonly SessionComponent m_SessionComponent;
		private readonly TaskComponent m_TaskComponent;

		public VibeBoardAppControl(VibeBoard parent, int id) : base(parent, id)
		{
			m_PackageComponent = parent.Components.GetComponent<PackageComponent>();

			m_StartComponent = parent.Components.GetComponent<StartComponent>();
			Subscribe(m_StartComponent);

			m_KeyComponent = parent.Components.GetComponent<KeyComponent>();

			m_SessionComponent = parent.Components.GetComponent<SessionComponent>();

			m_TaskComponent = parent.Components.GetComponent<TaskComponent>();
			Subscribe(m_TaskComponent);
		}

		protected override void DisposeFinal(bool disposing)
		{
			Unsubscribe(m_TaskComponent);
			Unsubscribe(m_StartComponent);

			base.DisposeFinal(disposing);
		}

		public void LaunchApp(eVibeApp app)
		{
			if (!IsInstalled(app))
			{
				OnAppLaunchFailed.Raise(this);
				return;
			}

			string packageName = GetPackageName(app);
			string activityName = GetActivityName(app);
			m_StartComponent.StartActivity(packageName, activityName);
		}

		public void PressKey(eVibeKey key)
		{
			m_KeyComponent.KeyPress(key);
		}

		public string GetPackageName(eVibeApp app)
		{
			if (!s_AppPackageNames.ContainsKey(app))
				throw new InvalidOperationException(string.Format("No package name exists for app {0}", app));

			return s_AppPackageNames[app];
		}

		public string GetActivityName(eVibeApp app)
		{
			if (!s_AppActivityNames.ContainsKey(app))
				throw new InvalidOperationException(string.Format("No activity name exists for app {0}", app));

			return s_AppActivityNames[app];
		}

		public bool IsInstalled(eVibeApp app)
		{
			string packageName = GetPackageName(app);
			return m_PackageComponent.Packages.Any(p => p.PackageName.Equals(packageName, StringComparison.OrdinalIgnoreCase));
		}

		public void EndSession()
		{
			m_SessionComponent.EndSession();
		}

		#region Component Callbacks

		private void Subscribe(StartComponent component)
		{
			component.OnAppLaunched += ComponentOnAppLaunched;
			component.OnAppLaunchFailed += ComponentOnAppLaunchFailed;
		}

		private void Unsubscribe(StartComponent component)
		{
			component.OnAppLaunched -= ComponentOnAppLaunched;
			component.OnAppLaunchFailed -= ComponentOnAppLaunchFailed;
		}

		private void ComponentOnAppLaunched(object sender, EventArgs e)
		{
			// check foreground task to confirm launch and populate OnAppLaunched event args
			m_TaskComponent.TopTask();
		}

		private void ComponentOnAppLaunchFailed(object sender, EventArgs e)
		{
			OnAppLaunchFailed.Raise(this);
		}

		private void Subscribe(TaskComponent component)
		{
			component.OnForegroundTaskUpdated += ComponentOnForegroundTaskUpdated;
		}

		private void Unsubscribe(TaskComponent component)
		{
			component.OnForegroundTaskUpdated -= ComponentOnForegroundTaskUpdated;
		}

		private void ComponentOnForegroundTaskUpdated(object sender, EventArgs eventArgs)
		{
			if (!m_TaskComponent.ForegroundTask.TopActivity.StartsWith(GetPackageName(eVibeApp.TouchCue)))
				OnAppLaunched.Raise(this);
		}

		#endregion
	}

	public enum eVibeApp
	{
		TouchCue,
		Chrome,
		Youtube,
		Slack,
		Whiteboard,
		Teams,
		WebEx
	}
}
