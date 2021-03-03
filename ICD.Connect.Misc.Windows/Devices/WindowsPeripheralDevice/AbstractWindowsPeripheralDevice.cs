using ICD.Connect.Devices;
using ICD.Connect.Misc.Windows.Devices.ControlSystems;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.Windows.Devices.WindowsPeripheralDevice
{
	public abstract class AbstractWindowsPeripheralDevice<TSettings> : AbstractDevice<TSettings>, IWindowsPeripheralDevice where TSettings : IWindowsPeripheralDeviceSettings, new()
	{
		/// <summary>
		/// Unique identifier for the peripheral device
		/// </summary>
		public string DeviceId { get; set; }

		/// <summary>
		/// The windows control system this device is attached to
		/// </summary>
		public WindowsControlSystem ControlSystem { get; set; }

		#region Settings

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			DeviceId = settings.DeviceId;
			ControlSystem = factory.GetOriginatorById<WindowsControlSystem>(settings.ControlSystem);

			ControlSystem.PeripheralComponent.RegisterPeripheral(this);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.DeviceId = DeviceId;
			settings.ControlSystem = ControlSystem.Id;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			ControlSystem.PeripheralComponent.DeregisterPeripheral(this);

			DeviceId = null;
			ControlSystem = null;
		}

		#endregion
	}
}
