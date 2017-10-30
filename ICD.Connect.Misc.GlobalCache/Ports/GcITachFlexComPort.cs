using System;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.GlobalCache.Devices;
using ICD.Connect.Misc.GlobalCache.FlexApi.RestApi;
using ICD.Connect.Protocol.Network.Tcp;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Protocol.Utils;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.GlobalCache.Ports
{
    public sealed class GcITachFlexComPort : AbstractComPort<GcITachFlexComPortSettings>
	{
		private const ushort PORT = 4999;

		private readonly AsyncTcpClient m_Client;

		private GcITachFlexDevice m_Device;
		private int m_Module;
		private int m_Address;

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
			bool output = m_Client.Send(data);
			PrintTx(data);
			return output;
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

		public override void SetComPortSpec(eComBaudRates baudRate, eComDataBits numberOfDataBits, eComParityType parityType,
											eComStopBits numberOfStopBits, eComProtocolType protocolType, eComHardwareHandshakeType hardwareHandShake,
											eComSoftwareHandshakeType softwareHandshake, bool reportCtsChanges)
		{
			if (m_Device == null)
				throw new InvalidOperationException(string.Format("{0} unable to connect - device is null", this));

			string localUrl = string.Format("api/host/modules/{0}/ports/{1}/config", m_Module, m_Address);

			SerialConfiguration config = new SerialConfiguration
			{
				BaudRate = ComSpecUtils.BaudRateToRate(baudRate),
				Parity = GetParity(parityType),
				StopBits = ComSpecUtils.StopBitsToCount(numberOfStopBits),
				FlowControl = hardwareHandShake == eComHardwareHandshakeType.ComspecHardwareHandshakeNone
								  ? SerialConfiguration.eFlowControl.None
								  : SerialConfiguration.eFlowControl.Hardware
			};

			m_Device.Post(localUrl, config.Serialize());
		}

		private SerialConfiguration.eParity GetParity(eComParityType parityType)
		{
			switch (parityType)
			{
				case eComParityType.ComspecParityNone:
					return SerialConfiguration.eParity.None;
				case eComParityType.ComspecParityEven:
					return SerialConfiguration.eParity.Even;
				case eComParityType.ComspecParityOdd:
					return SerialConfiguration.eParity.Odd;

				default:
					throw new ArgumentOutOfRangeException("parityType");
			}
		}

		/// <summary>
		/// Sets the parent device.
		/// </summary>
		/// <param name="device"></param>
		[PublicAPI]
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
			return m_Client != null && m_Client.IsConnected;
		}

		protected override bool GetIsOnlineStatus()
		{
			return m_Client != null && m_Client.IsOnline;
		}

		#region TCP Client Callbacks

		/// <summary>
		/// Subscribe to the client events.
		/// </summary>
		/// <param name="client"></param>
		private void Subscribe(AsyncTcpClient client)
		{
			client.OnIsOnlineStateChanged += ClientOnOnIsOnlineStateChanged;
			client.OnConnectedStateChanged += ClientOnOnConnectedStateChanged;
			client.OnSerialDataReceived += ClientOnOnSerialDataReceived;
		}

		/// <summary>
		/// Unsubscribe from the client events.
		/// </summary>
		/// <param name="client"></param>
		private void Unsubscribe(AsyncTcpClient client)
		{
			client.OnIsOnlineStateChanged -= ClientOnOnIsOnlineStateChanged;
			client.OnConnectedStateChanged -= ClientOnOnConnectedStateChanged;
			client.OnSerialDataReceived -= ClientOnOnSerialDataReceived;
		}

		/// <summary>
		/// Called when we get data from the tcp client.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void ClientOnOnSerialDataReceived(object sender, StringEventArgs stringEventArgs)
		{
			PrintRx(stringEventArgs.Data);
			Receive(stringEventArgs.Data);
		}

		/// <summary>
		/// Called when the TCP client connects/disconnects.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ClientOnOnConnectedStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateIsConnectedState();
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

			m_Module = 1;
			m_Address = 1;

			SetDevice(null);
		}

		protected override void CopySettingsFinal(GcITachFlexComPortSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Module = m_Module;
			settings.Address = m_Address;
			settings.Device = m_Device == null ? (int?)null : m_Device.Id;
		}

		protected override void ApplySettingsFinal(GcITachFlexComPortSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			GcITachFlexDevice device = null;

			if (settings.Device != null)
			{
				device = factory.GetDeviceById((int)settings.Device) as GcITachFlexDevice;
				if (device == null)
					Logger.AddEntry(eSeverity.Error, "{0} is not a {1}", m_Device, typeof(GcITachFlexDevice).Name);
			}

			m_Module = settings.Module;
			m_Address = settings.Address;
			SetDevice(device);
		}

		#endregion
	}
}
