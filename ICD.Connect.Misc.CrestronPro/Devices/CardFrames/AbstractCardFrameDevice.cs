using ICD.Common.Utils.Services.Logging;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
#endif
using ICD.Connect.Devices;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Devices.CardFrames
{
#if SIMPLSHARP
	public abstract class AbstractCardFrameDevice<TCardFrame, TSettings> : AbstractDevice<TSettings>, ICardFrameDevice
		where TCardFrame : GenericDevice
#else
	public abstract class AbstractCardFrameDevice<TSettings> : AbstractDevice<TSettings>, ICardFrameDevice
#endif
		where TSettings : ICardFrameDeviceSettings, new()
	{
#if SIMPLSHARP
		public TCardFrame CardFrame { get; private set; }
#endif

		#region Methods

#if SIMPLSHARP
		/// <summary>
		/// Sets the wrapped device.
		/// </summary>
		/// <param name="device"></param>
		public void SetCardCage(TCardFrame device)
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
#endif

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if SIMPLSHARP
			return CardFrame != null && CardFrame.IsOnline;
#else
            return false;
#endif
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

#if SIMPLSHARP
			settings.Ipid = CardFrame == null ? (byte)0 : (byte)CardFrame.ID;
#else
            settings.Ipid = 0;
#endif
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

#if SIMPLSHARP
			SetCardCage(null);
#endif
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

#if SIMPLSHARP
			TCardFrame device = Instantiate(settings.Ipid, ProgramInfo.ControlSystem);
			SetCardCage(device);
#else
            throw new System.NotImplementedException();
#endif
		}

#if SIMPLSHARP
		/// <summary>
		/// Creates a new card frame with the given parameters.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="controlSystem"></param>
		/// <returns></returns>
		protected abstract TCardFrame Instantiate(byte ipid, CrestronControlSystem controlSystem);
#endif

		#endregion

		#region Device Callbacks

#if SIMPLSHARP
		/// <summary>
		/// Subscribe to the device events.
		/// </summary>
		/// <param name="portsDevice"></param>
		private void Subscribe(TCardFrame portsDevice)
		{
			if (portsDevice == null)
				return;

			portsDevice.OnlineStatusChange += PortsDeviceOnLineStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="portsDevice"></param>
		private void Unsubscribe(TCardFrame portsDevice)
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
#endif

		#endregion
	}
}