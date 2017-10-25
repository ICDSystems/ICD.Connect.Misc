using System;
using System.Collections.Generic;
using ICD.Connect.Misc.CrestronPro.Utils;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
#else
using System;
#endif
using ICD.Common.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Devices
{
#if SIMPLSHARP
    public sealed class DinIo8Adapter : AbstractDevice<DinIo8AdapterSettings>, IPortParent
#else
    public sealed class DinIo8Adapter : AbstractDevice<DinIo8AdapterSettings>
#endif
    {
#if SIMPLSHARP
        private DinIo8 m_PortsDevice;
#endif

#region Methods

#if SIMPLSHARP
        /// <summary>
        /// Sets the wrapped device.
        /// </summary>
        /// <param name="device"></param>
        public void SetDevice(DinIo8 device)
		{
			if (device == m_PortsDevice)
				return;

			Unsubscribe(m_PortsDevice);

			if (m_PortsDevice != null)
			{
				if (m_PortsDevice.Registered)
					m_PortsDevice.UnRegister();

				try
				{
					m_PortsDevice.Dispose();
				}
				catch
				{
				}
			}

			m_PortsDevice = device;

			if (m_PortsDevice != null && !m_PortsDevice.Registered)
			{
				if (Name != null)
					m_PortsDevice.Description = Name;
				eDeviceRegistrationUnRegistrationResponse result = m_PortsDevice.Register();
				if (result != eDeviceRegistrationUnRegistrationResponse.Success)
					Logger.AddEntry(eSeverity.Error, "Unable to register {0} - {1}", m_PortsDevice.GetType().Name, result);
			}

			Subscribe(m_PortsDevice);
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
            return m_PortsDevice != null && m_PortsDevice.IsOnline;
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
        public ComPort GetComPort(int address)
		{
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(ComPort).Name, address);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public IROutputPort GetIrOutputPort(int address)
		{
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(IROutputPort).Name, address);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public Relay GetRelayPort(int address)
		{
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(Relay).Name, address);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public Versiport GetIoPort(int address)
		{
			if (m_PortsDevice == null)
				throw new InvalidOperationException(string.Format("{0} has no internal device", this));

			if (m_PortsDevice.VersiPorts.Contains((uint)address))
				return m_PortsDevice.VersiPorts[(uint)address];

			string message = string.Format("{0} has no {1} with address {2}", this, typeof(Versiport).Name, address);
			throw new KeyNotFoundException(message);
		}

	    /// <summary>
	    /// Gets the port at the given address.
	    /// </summary>
	    /// <param name="address"></param>
	    /// <returns></returns>
	    public DigitalInput GetDigitalInputPort(int address)
	    {
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(DigitalInput).Name, address);
			throw new KeyNotFoundException(message);
	    }
#endif

#endregion

#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(DinIo8AdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

#if SIMPLSHARP
            settings.CresnetId = m_PortsDevice == null ? (byte)0 : (byte)m_PortsDevice.ID;
#else
            settings.CresnetId = 0;
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
	    protected override void ApplySettingsFinal(DinIo8AdapterSettings settings, IDeviceFactory factory)
	    {
		    base.ApplySettingsFinal(settings, factory);

#if SIMPLSHARP
		    if (!CresnetUtils.IsValidId(settings.CresnetId))
		    {
			    Logger.AddEntry(eSeverity.Error, "{0} failed to instantiate {1} - CresnetId {2} is out of range",
			                    GetType().Name, typeof(DinIo8).Name, settings.CresnetId);
			    return;
		    }

		    DinIo8 device = null;

		    try
		    {
				device = new DinIo8(settings.CresnetId, ProgramInfo.ControlSystem);
		    }
		    catch (ArgumentException e)
		    {
			    string message = string.Format("Failed to instantiate {0} with Cresnet ID {1} - {2}",
			                                   typeof(DinIo8).Name, settings.CresnetId, e.Message);
			    Logger.AddEntry(eSeverity.Error, e, message);
		    }
		    
		    SetDevice(device);
#else
            throw new NotImplementedException();
#endif
	    }

	    #endregion

#region Device Callbacks

#if SIMPLSHARP
        /// <summary>
        /// Subscribe to the device events.
        /// </summary>
        /// <param name="portsDevice"></param>
        private void Subscribe(DinIo8 portsDevice)
		{
			if (portsDevice == null)
				return;

			portsDevice.OnlineStatusChange += PortsDeviceOnLineStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="portsDevice"></param>
		private void Unsubscribe(DinIo8 portsDevice)
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
