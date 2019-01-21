using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Protocol;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports;
using ICD.Connect.Protocol.Network.Ports.Tcp;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.SerialPortServerDevice
{
	public sealed class SerialPortServerDevice : AbstractDevice<SerialPortServerDeviceSettings>
	{
		#region Fields
		private readonly ComSpecProperties m_ComSpecProperties;
		private readonly SecureNetworkProperties m_NetworkProperties;
		private readonly ConnectionStateManager m_ConnectionStateManager;

		private AsyncTcpServer m_TcpServer;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the com spec properties.
		/// </summary>
		private IComSpecProperties ComSpecProperties { get { return m_ComSpecProperties; } }

		/// <summary>
		/// Gets the network properties.
		/// </summary>
		private ISecureNetworkProperties NetworkProperties { get { return m_NetworkProperties; } }

		private ushort? TcpServerPort {get { return m_TcpServer != null ? m_TcpServer.Port : (ushort?)null; } }

		#endregion

		public SerialPortServerDevice()
		{
			m_NetworkProperties = new SecureNetworkProperties();
			m_ComSpecProperties = new ComSpecProperties();

			m_ConnectionStateManager = new ConnectionStateManager(this)
			{
				ConfigurePort = ConfigurePort
			};
			m_ConnectionStateManager.OnIsOnlineStateChanged += CsmOnIsOnlineStateChanged;
			m_ConnectionStateManager.OnConnectedStateChanged += CsmOnConnectedStateChanged;
			m_ConnectionStateManager.OnSerialDataReceived += CsmOnSerialDataReceived;
		}

		public void ConfigurePort(ISerialPort port)
		{
			// Com
			if (port is IComPort)
				(port as IComPort).ApplyDeviceConfiguration(ComSpecProperties);

			// Network (TCP, UDP, SSH)
			if (port is ISecureNetworkPort)
				(port as ISecureNetworkPort).ApplyDeviceConfiguration(NetworkProperties);
			else if (port is INetworkPort)
				(port as INetworkPort).ApplyDeviceConfiguration(NetworkProperties);
		}

		#region Port

		/// <summary>
		/// Sets and configures the port for communication with the physical display.
		/// </summary>
		[PublicAPI]
		public void SetPort(ISerialPort port)
		{
			m_ConnectionStateManager.SetPort(port);
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_ConnectionStateManager.Port != null && m_ConnectionStateManager.Port.IsOnline;
		}

		#endregion


		#region ConnectionStateManager

		/// <summary>
		/// Send received port data to all connected clients
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void CsmOnSerialDataReceived(object sender, StringEventArgs stringEventArgs)
		{
			if (m_TcpServer != null)
				m_TcpServer.Send(stringEventArgs.Data);
		}

		private void CsmOnIsOnlineStateChanged(object sender, BoolEventArgs e)
		{
			UpdateCachedOnlineStatus();
		}

		private void CsmOnConnectedStateChanged(object sender, BoolEventArgs e)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region TCP Server

		private void Subscribe(AsyncTcpServer tcpServer)
		{
			if (tcpServer == null)
				return;

			tcpServer.OnDataReceived += IncomingServerOnDataReceived;
		}

		private void Unsubscribe(AsyncTcpServer tcpServer)
		{
			if (tcpServer == null)
				return;

			tcpServer.OnDataReceived -= IncomingServerOnDataReceived;
		}

		/// <summary>
		/// Send received server data to port
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="tcpReceiveEventArgs"></param>
		private void IncomingServerOnDataReceived(object sender, TcpReceiveEventArgs tcpReceiveEventArgs)
		{
			m_ConnectionStateManager.Send(tcpReceiveEventArgs.Data);
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(SerialPortServerDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Port = m_ConnectionStateManager.PortNumber;
			settings.TcpServerPort = TcpServerPort;

			settings.Copy(m_ComSpecProperties);
			settings.Copy(m_NetworkProperties);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetPort(null);

			m_TcpServer.Stop();
			Unsubscribe(m_TcpServer);
			m_TcpServer = null;

			m_ComSpecProperties.ClearComSpecProperties();
			m_NetworkProperties.ClearNetworkProperties();
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(SerialPortServerDeviceSettings settings, IDeviceFactory factory)
		{

			base.ApplySettingsFinal(settings, factory);

			m_NetworkProperties.Copy(settings);
			m_ComSpecProperties.Copy(settings);

			ISerialPort port = null;

			if (settings.Port != null)
			{
				try
				{
					port = factory.GetPortById((int)settings.Port) as ISerialPort;
				}
				catch (KeyNotFoundException)
				{
					Log(eSeverity.Error, "No Serial Port with id {0}", settings.Port);
				}
			}

			SetPort(port);
			UpdateCachedOnlineStatus();

			if (settings.TcpServerPort == null)
			{
				Log(eSeverity.Error, "TCP Server port not specified in config");
			}
			else
			{
				m_TcpServer = new AsyncTcpServer();
				Subscribe(m_TcpServer);
				m_TcpServer.Name = String.Format("{0} TCP Server", settings.Name);
				m_TcpServer.MaxNumberOfClients = 10;
				m_TcpServer.Port = settings.TcpServerPort.Value;
				m_TcpServer.Start();
			}
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("TCP Server Port", TcpServerPort);
		}

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			if (m_TcpServer != null)
				yield return m_TcpServer;
		}

		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		#endregion
	}
}