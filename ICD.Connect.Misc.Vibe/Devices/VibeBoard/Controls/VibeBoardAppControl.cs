using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components;

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
			{eVibeApps.Slack, "com.Slack"},
			{eVibeApps.Whiteboard, "ai.inlight.board.app" }
		};
		private static readonly Dictionary<eVibeApps, string> m_AppActivityNames = new Dictionary<eVibeApps, string>()
		{
			{eVibeApps.Chrome, "com.google.android.apps.chrome.Main"},
			{eVibeApps.Youtube, ".app.honeycomb.Shell$HomeActivity"},
			{eVibeApps.Slack, ".ui.HomeActivity"},
			{eVibeApps.Whiteboard, "ai.inlight.board.app.MainActivity" }
		};

		public VibeBoardAppControl(VibeBoard parent, int id) : base(parent, id)
		{
			m_PackageComponent = parent.Components.GetComponent<PackageComponent>();
			Subscribe(m_PackageComponent);

			m_StartComponent = parent.Components.GetComponent<StartComponent>();
			Subscribe(m_StartComponent);
		}

		protected override void DisposeFinal(bool disposing)
		{
			Unsubscribe(m_PackageComponent);
			Unsubscribe(m_StartComponent);

			base.DisposeFinal(disposing);
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
			component.OnPackagesUpdated += ComponentOnPackagesUpdated;
		}

		private void Unsubscribe(PackageComponent component)
		{
			component.OnPackagesUpdated -= ComponentOnPackagesUpdated;
		}

		private void ComponentOnPackagesUpdated(object sender, EventArgs e)
		{
			// todo do something eventually
		}

		#endregion
	}

	public enum eVibeApps
	{
		Chrome,
		Youtube,
		Slack,
		Whiteboard,
	}
}
