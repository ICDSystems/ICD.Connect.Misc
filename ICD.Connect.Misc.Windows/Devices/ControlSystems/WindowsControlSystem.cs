using System;
using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Misc.ControlSystems;
using ICD.Connect.Settings;
using ICD.Connect.Misc.Windows.WindowsPeripheral;
#if !SIMPLSHARP
using ICD.Connect.Misc.Windows.Utils;
#endif

namespace ICD.Connect.Misc.Windows.Devices.ControlSystems
{
	public sealed class WindowsControlSystem : AbstractControlSystemDevice<WindowsControlSystemSettings>
	{
		private readonly WindowsPeripheralComponent m_PeripheralComponent;

		public string PeripheralWhitelistConfig { get; private set; }

		public WindowsPeripheralComponent PeripheralComponent { get { return m_PeripheralComponent; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public WindowsControlSystem()
		{
			MonitoredDeviceInfo.Make = "Microsoft";
			m_PeripheralComponent = new WindowsPeripheralComponent(this);
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return true;
		}

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(WindowsControlSystemSettings settings, IDeviceFactory factory,
											Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new WindowsControlSystemRoutingControl(this, 0));
			addControl(new WindowsControlSystemMasterVolumeControl(this, 1));
		}

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			PeripheralWhitelistConfig = null;

			PeripheralComponent.Clear();
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(WindowsControlSystemSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.PeripheralWhitelist = PeripheralWhitelistConfig;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(WindowsControlSystemSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			PeripheralWhitelistConfig = settings.PeripheralWhitelist;

			if (!string.IsNullOrEmpty(PeripheralWhitelistConfig))
				PeripheralComponent.LoadPeripheralConfig(PeripheralWhitelistConfig);
		}

		/// <summary>
		/// Override to add actions on StartSettings
		/// This should be used to start communications with devices and perform initial actions
		/// </summary>
		protected override void StartSettingsFinal()
		{
			base.StartSettingsFinal();

			PeripheralComponent.UpdatePeripherals();
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

#if !SIMPLSHARP
			yield return new GenericConsoleCommand<string, string>("SwitchUser", "SwitchUser <USERNAME> <PASSWORD>",
			                                                       (u, p) => LogonUtils.SwitchUser(u, p));
#endif
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}
		#endregion
	}
}
