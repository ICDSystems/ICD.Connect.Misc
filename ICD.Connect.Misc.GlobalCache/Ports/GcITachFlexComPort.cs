using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Misc.GlobalCache.Devices;
using ICD.Connect.Protocol.Network.Tcp;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.GlobalCache.Ports
{
    public sealed class GcITachFlexComPort : AbstractComPort<GcITachFlexComPortSettings>
	{
		private const ushort PORT = 4999;

		private readonly AsyncTcpClient m_Client;

		private GcITachFlexDevice m_Device;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachFlexComPort()
		{
			m_Client = new AsyncTcpClient();
			Subscribe(m_Client);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(m_Client);
		}

		#region Methods

		/// <summary>
		/// Sends data to the device.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected override bool SendFinal(string data)
		{
			return m_Client.Send(data);
		}

		/// <summary>
		/// Connects to the device.
		/// </summary>
		public override void Connect()
		{
			if (m_Device == null)
				throw new InvalidOperationException(string.Format("{0} unable to connect - device is null", this));

			HostInfo host = new HostInfo(m_Device.Address, PORT);
			m_Client.Connect(host);
		}

		/// <summary>
		/// Disconnects from the device.
		/// </summary>
		public override void Disconnect()
		{
			m_Client.Disconnect();
		}

		public override int SetComPortSpec(eComBaudRates baudRate, eComDataBits numberOfDataBits, eComParityType parityType,
										   eComStopBits numberOfStopBits, eComProtocolType protocolType, eComHardwareHandshakeType hardwareHandShake,
										   eComSoftwareHandshakeType softwareHandshake, bool reportCtsChanges)
		{
			if (m_Device == null)
				throw new InvalidOperationException(string.Format("{0} unable to connect - device is null", this));

			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Sets the parent device.
		/// </summary>
		/// <param name="device"></param>
		public void SetDevice(GcITachFlexDevice device)
		{
			if (device == m_Device)
				return;

			Disconnect();

			m_Device = device;
		}

		#endregion

		protected override bool GetIsConnectedState()
		{
			return m_Client.IsConnected;
		}

		protected override bool GetIsOnlineStatus()
		{
			return m_Client.IsOnline;
		}

		#region TCP Client Callbacks

		/// <summary>
		/// Subscribe to the client events.
		/// </summary>
		/// <param name="client"></param>
		private void Subscribe(AsyncTcpClient client)
		{
			client.OnIsOnlineStateChanged += ClientOnOnIsOnlineStateChanged;
			client.OnSerialDataReceived += ClientOnOnSerialDataReceived;
		}

		/// <summary>
		/// Unsubscribe from the client events.
		/// </summary>
		/// <param name="client"></param>
		private void Unsubscribe(AsyncTcpClient client)
		{
			client.OnIsOnlineStateChanged -= ClientOnOnIsOnlineStateChanged;
			client.OnSerialDataReceived -= ClientOnOnSerialDataReceived;
		}

		/// <summary>
		/// Called when we get data from the tcp client.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void ClientOnOnSerialDataReceived(object sender, StringEventArgs stringEventArgs)
		{
			Receive(stringEventArgs.Data);
		}

		/// <summary>
		/// Called when the TCP client changes online status.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ClientOnOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Settings

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetDevice(null);
		}

		protected override void CopySettingsFinal(GcITachFlexComPortSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Device = m_Device == null ? (int?)null : m_Device.Id;
		}

		protected override void ApplySettingsFinal(GcITachFlexComPortSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			throw new NotImplementedException();
		}

		#endregion
	}
}
