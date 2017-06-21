using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
using ICD.Common.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Devices
{
	public sealed class DinIo8Adapter : AbstractDevice<DinIo8AdapterSettings>, IPortParent
	{
		private DinIo8 m_PortsDevice;

		#region Methods

		/// <summary>
		/// Sets the wrapped device.
		/// </summary>
		/// <param name="device"></param>
		public void SetPortsDevice(DinIo8 device)
		{
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

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_PortsDevice != null && m_PortsDevice.IsOnline;
		}

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
			if (m_PortsDevice.VersiPorts.Contains((uint)address))
				return m_PortsDevice.VersiPorts[1];

			string message = string.Format("{0} has no {1} with address {2}", this, typeof(Versiport).Name, address);
			throw new KeyNotFoundException(message);
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(DinIo8AdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Ipid = m_PortsDevice == null ? (byte)0 : (byte)m_PortsDevice.ID;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetPortsDevice(null);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(DinIo8AdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			DinIo8 device = new DinIo8(settings.Ipid, ProgramInfo.ControlSystem);
			SetPortsDevice(device);
		}

		#endregion

		#region Device Callbacks

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

		#endregion
	}
}
