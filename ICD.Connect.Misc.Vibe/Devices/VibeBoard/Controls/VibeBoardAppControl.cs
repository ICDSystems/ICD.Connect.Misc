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

		private static readonly Dictionary<eVibeApps, string> s_AppPackageNames = new Dictionary<eVibeApps, string>()
		{
			{eVibeApps.Chrome, "com.android.chrome"},
			{eVibeApps.Youtube, "com.google.android.youtube"},
			{eVibeApps.Slack, "com.Slack"},
			{eVibeApps.Whiteboard, "ai.inlight.board.app"},
			{eVibeApps.Teams, "com.microsoft.teams"},
			{eVibeApps.WebEx, "com.cisco.webex.meetings"}
		};
		private static readonly Dictionary<eVibeApps, string> s_AppActivityNames = new Dictionary<eVibeApps, string>()
		{
			{eVibeApps.Chrome, "com.google.android.apps.chrome.Main"},
			{eVibeApps.Youtube, ".app.honeycomb.Shell$HomeActivity"},
			{eVibeApps.Slack, ".ui.HomeActivity"},
			{eVibeApps.Whiteboard, "ai.inlight.board.app.MainActivity" },
			{eVibeApps.Teams, "com.microsoft.skype.teams.views.activities.SplashActivity"},
			{eVibeApps.WebEx, "com.cisco.webex.meetings.ui.premeeting.welcome.WebExMeeting"}
		};
		
		private readonly PackageComponent m_PackageComponent;
		private readonly StartComponent m_StartComponent;
		private readonly KeyComponent m_KeyComponent;
		private readonly SessionComponent m_SessionComponent;

		public VibeBoardAppControl(VibeBoard parent, int id) : base(parent, id)
		{
			m_PackageComponent = parent.Components.GetComponent<PackageComponent>();
			Subscribe(m_PackageComponent);

			m_StartComponent = parent.Components.GetComponent<StartComponent>();
			Subscribe(m_StartComponent);

			m_KeyComponent = parent.Components.GetComponent<KeyComponent>();

			m_SessionComponent = parent.Components.GetComponent<SessionComponent>();
		}

		protected override void DisposeFinal(bool disposing)
		{
			Unsubscribe(m_PackageComponent);
			Unsubscribe(m_StartComponent);

			base.DisposeFinal(disposing);
		}

		public void LaunchApp(eVibeApps app)
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

		public string GetPackageName(eVibeApps app)
		{
			if (!s_AppPackageNames.ContainsKey(app))
				throw new InvalidOperationException(string.Format("No package name exists for app {0}", app));

			return s_AppPackageNames[app];
		}

		public string GetActivityName(eVibeApps app)
		{
			if (!s_AppActivityNames.ContainsKey(app))
				throw new InvalidOperationException(string.Format("No activity name exists for app {0}", app));

			return s_AppActivityNames[app];
		}

		public bool IsInstalled(eVibeApps app)
		{
			string packageName = GetPackageName(app);
			return m_PackageComponent.Packages.Any(p => p.PackageName.Equals(packageName, StringComparison.OrdinalIgnoreCase));
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
			OnAppLaunched.Raise(this);
		}

		private void ComponentOnAppLaunchFailed(object sender, EventArgs e)
		{
			OnAppLaunchFailed.Raise(this);
		}

		private void Subscribe(PackageComponent component)
		{
			// nothing for now
		}

		private void Unsubscribe(PackageComponent component)
		{
			// nothing for now
		}

		#endregion
	}

	public enum eVibeApps
	{
		Chrome,
		Youtube,
		Slack,
		Whiteboard,
		Teams,
		WebEx
	}
}
