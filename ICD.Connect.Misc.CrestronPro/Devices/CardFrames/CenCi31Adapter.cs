using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.ThreeSeriesCards;
using ICD.Common.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Devices.CardFrames
{
	public sealed class CenCi31Adapter : AbstractDevice<CenCi31AdapterSettings>
	{
		public CenCi31 CardFrame { get; private set; }

		#region Methods

		/// <summary>
		/// Sets the wrapped device.
		/// </summary>
		/// <param name="device"></param>
		public void SetCardCage(CenCi31 device)
		{
			Unsubscribe(CardFrame);

			if (CardFrame != null)
			{
				if (CardFrame.Registered)
					CardFrame.UnRegister();

				try
				{
					CardFrame.Dispose();
				}
				catch
				{
				}
			}

			CardFrame = device;

			if (CardFrame != null && !CardFrame.Registered)
			{
				if (Name != null)
					CardFrame.Description = Name;
				eDeviceRegistrationUnRegistrationResponse result = CardFrame.Register();
				if (result != eDeviceRegistrationUnRegistrationResponse.Success)
					Logger.AddEntry(eSeverity.Error, "Unable to register {0} - {1}", CardFrame.GetType().Name, result);
			}

			Subscribe(CardFrame);
			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return CardFrame != null && CardFrame.IsOnline;
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(CenCi31AdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Ipid = CardFrame == null ? (byte)0 : (byte)CardFrame.ID;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetCardCage(null);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(CenCi31AdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			CenCi31 device = new CenCi31(settings.Ipid, ProgramInfo.ControlSystem);
			SetCardCage(device);
		}

		#endregion

		#region Device Callbacks

		/// <summary>
		/// Subscribe to the device events.
		/// </summary>
		/// <param name="portsDevice"></param>
		private void Subscribe(CenCi31 portsDevice)
		{
			if (portsDevice == null)
				return;

			portsDevice.OnlineStatusChange += PortsDeviceOnLineStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="portsDevice"></param>
		private void Unsubscribe(CenCi31 portsDevice)
		{
			if (portsDevice == null)
				return;

			portsDevice.OnlineStatusChange -= PortsDeviceOnLineStatusChange;
		}

		/// <summary>
		/// Called when the device online status changes.
		/// </summary>
		/// <param name="currentDevice"></param>
		/// <param name="args"></param>
		private void PortsDeviceOnLineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion
	}
}
