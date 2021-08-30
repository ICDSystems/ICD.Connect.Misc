using System;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
#endif
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
#if !NETSTANDARD
	public abstract class AbstractCenIoAdapter<TDevice, TSettings> : AbstractDevice<TSettings>, ICenIoAdapter
		where TDevice : GenericDevice
#else
	public abstract class AbstractCenIoAdapter<TSettings> : AbstractDevice<TSettings>, ICenIoAdapter
#endif
		where TSettings : ICenIoAdapterSettings, new()
	{
#if !NETSTANDARD
		public TDevice Device { get; private set; }
#endif

		#region Methods

#if !NETSTANDARD
		/// <summary>
		/// Sets the wrapped device.
		/// </summary>
		/// <param name="device"></param>
		public void SetDevice(TDevice device)
		{
			if (device == Device)
				return;

			Unsubscribe(Device);

			if (Device != null)
				GenericBaseUtils.TearDown(Device);

			Device = device;

			eDeviceRegistrationUnRegistrationResponse result;
			if (Device != null && !GenericBaseUtils.SetUp(Device, this, out result))
				Logger.Log(eSeverity.Error, "Unable to register {0} - {1}", Device.GetType().Name, result);

			Subscribe(Device);
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
			return Device != null && Device.IsOnline;
#else
			return false;
#endif
		}

#if !NETSTANDARD
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual ComPort GetComPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(ComPort).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual IROutputPort GetIrOutputPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(IROutputPort).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual Relay GetRelayPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Relay).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual Versiport GetIoPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Versiport).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual DigitalInput GetDigitalInputPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(DigitalInput).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="io"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual Cec GetCecPort(eInputOuptut io, int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Cec).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}
#endif

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
			settings.Ipid = Device == null ? (byte)0 : (byte)Device.ID;
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
			SetDevice(null);
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
			TDevice device = InstantiateDevice(settings);
			SetDevice(device);
#endif
		}

#if !NETSTANDARD
		/// <summary>
		/// Creates a new instance of the wrapped internal device.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		protected abstract TDevice InstantiateDevice(TSettings settings);
#endif

		#endregion

		#region Device Callbacks

#if !NETSTANDARD
		/// <summary>
		/// Subscribe to the device events.
		/// </summary>
		/// <param name="device"></param>
		private void Subscribe(TDevice device)
		{
			if (device == null)
				return;

			device.OnlineStatusChange += PortsDeviceOnLineStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="device"></param>
		private void Unsubscribe(TDevice device)
		{
			if (device == null)
				return;

			device.OnlineStatusChange -= PortsDeviceOnLineStatusChange;
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
