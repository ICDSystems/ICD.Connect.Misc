using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.CardFrames
{
#if !NETSTANDARD
	public abstract class AbstractCardFrameDevice<TCardFrame, TSettings> : AbstractDevice<TSettings>, ICardFrameDevice
		where TCardFrame : GenericDevice
#else
	public abstract class AbstractCardFrameDevice<TSettings> : AbstractDevice<TSettings>, ICardFrameDevice
#endif
		where TSettings : ICardFrameDeviceSettings, new()
	{
#if !NETSTANDARD
		public TCardFrame CardFrame { get; private set; }
#endif

		#region Methods

#if !NETSTANDARD
		/// <summary>
		/// Sets the wrapped device.
		/// </summary>
		/// <param name="device"></param>
		public void SetCardCage(TCardFrame device)
		{
			if (device == CardFrame)
				return;

			Unsubscribe(CardFrame);

			if (CardFrame != null)
				GenericBaseUtils.TearDown(CardFrame);

			CardFrame = device;

			eDeviceRegistrationUnRegistrationResponse result;
			if (CardFrame != null && !GenericBaseUtils.SetUp(CardFrame, this, out result))
				Logger.Log(eSeverity.Error, "Unable to register {0} - {1}", CardFrame.GetType().Name, result);

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
#if !NETSTANDARD
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

#if !NETSTANDARD
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

#if !NETSTANDARD
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

#if !NETSTANDARD
			TCardFrame device = settings.Ipid == null
								? null 
								: Instantiate(settings.Ipid.Value, ProgramInfo.ControlSystem);



			SetCardCage(device);
#endif
		}

#if !NETSTANDARD
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

#if !NETSTANDARD
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
