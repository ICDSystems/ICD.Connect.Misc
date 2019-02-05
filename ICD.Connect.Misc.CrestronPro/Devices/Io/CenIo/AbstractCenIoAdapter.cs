using System;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
#endif
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
#if SIMPLSHARP
	public abstract class AbstractCenIoAdapter<TDevice, TSettings> : AbstractDevice<TSettings>, ICenIoAdapter
		where TDevice : GenericDevice
#else
	public abstract class AbstractCenIoAdapter<TSettings> : AbstractDevice<TSettings>, ICenIoAdapter
#endif
		where TSettings : ICenIoAdapterSettings, new()
	{
#if SIMPLSHARP
		public TDevice Device { get; private set; }
#endif

#region Methods

#if SIMPLSHARP
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
			{
				if (Device.Registered)
					Device.UnRegister();

				try
				{
					Device.Dispose();
				}
				catch
				{
				}
			}

			Device = device;

			if (Device != null && !Device.Registered)
			{
				if (Name != null)
					Device.Description = Name;
				eDeviceRegistrationUnRegistrationResponse result = Device.Register();
				if (result != eDeviceRegistrationUnRegistrationResponse.Success)
					Log(eSeverity.Error, "{0} failed to register {1} - {2}", this, Device.GetType().Name, result);
			}

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
#if SIMPLSHARP
			return Device != null && Device.IsOnline;
#else
			return false;
#endif
		}

#if SIMPLSHARP
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

#if SIMPLSHARP
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

#if SIMPLSHARP
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

#if SIMPLSHARP
			TDevice device = InstantiateDevice(settings);
			SetDevice(device);
#endif
		}

#if SIMPLSHARP
		/// <summary>
		/// Creates a new instance of the wrapped internal device.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		protected abstract TDevice InstantiateDevice(TSettings settings);
#endif

#endregion

#region Device Callbacks

#if SIMPLSHARP
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
