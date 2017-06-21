using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Devices;
using ICD.Connect.Protocol;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Tcp;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Power.MiddleAtlantic
{
	public sealed class RackLinkDevice : AbstractDevice<RackLinkDeviceSettings>
	{
		// How often to check the connection and reconnect if necessary.
		private const long CONNECTION_CHECK_MILLISECONDS = 30 * 1000;

		/// <summary>
		/// Raised when the class initializes.
		/// </summary>
		[PublicAPI]
		public event EventHandler<BoolEventArgs> OnInitializedChanged;

		/// <summary>
		/// Raised when the device becomes connected or disconnected.
		/// </summary>
		[PublicAPI]
		public event EventHandler<BoolEventArgs> OnConnectedStateChanged;

		private readonly SafeTimer m_ConnectionTimer;

		private ISerialPort m_Port;
		private bool m_IsConnected;
		private bool m_Initialized;

		#region Properties

		/// <summary>
		/// Returns true when the device is connected.
		/// </summary>
		[PublicAPI]
		public bool IsConnected
		{
			get { return m_IsConnected; }
			private set
			{
				if (value == m_IsConnected)
					return;

				m_IsConnected = value;

				UpdateCachedOnlineStatus();

				OnConnectedStateChanged.Raise(this, new BoolEventArgs(m_IsConnected));
			}
		}

		/// <summary>
		/// Device Initialized Status.
		/// </summary>
		[PublicAPI]
		public bool Initialized
		{
			get { return m_Initialized; }
			private set
			{
				if (value == m_Initialized)
					return;

				m_Initialized = value;

				OnInitializedChanged.Raise(this, new BoolEventArgs(m_Initialized));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public RackLinkDevice()
		{
			m_ConnectionTimer = new SafeTimer(ConnectionTimerCallback, 0, CONNECTION_CHECK_MILLISECONDS);
		}

		#region Methods

		/// <summary>
		/// Sets the port used to communicate with the hardware.
		/// </summary>
		/// <param name="port"></param>
		[PublicAPI]
		public void SetPort(ISerialPort port)
		{
			if (port == m_Port)
				return;

			Unsubscribe(m_Port);

			m_Port = port;

			if (m_Port is IComPort)
				ConfigureComPort(m_Port as IComPort);
			if (m_Port is AsyncTcpClient)
				ConfigureTcpClient(m_Port as AsyncTcpClient);
			//m_SerialQueue.SetPort(m_Port);

			Subscribe(m_Port);

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Configures the comport for communication with the device.
		/// </summary>
		/// <param name="comPort"></param>
		[PublicAPI]
		public static void ConfigureComPort(IComPort comPort)
		{
			if (comPort == null)
				throw new ArgumentNullException("comPort");

			comPort.SetComPortSpec(eComBaudRates.ComspecBaudRate9600,
			                       eComDataBits.ComspecDataBits8,
			                       eComParityType.ComspecParityNone,
			                       eComStopBits.ComspecStopBits1,
			                       eComProtocolType.ComspecProtocolRS232,
			                       eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
			                       eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
			                       false);
		}

		/// <summary>
		/// Configures the tcp client for communication with the device.
		/// </summary>
		/// <param name="tcpClient"></param>
		[PublicAPI]
		public void ConfigureTcpClient(AsyncTcpClient tcpClient)
		{
			if (tcpClient == null)
				throw new ArgumentNullException("tcpClient");

			tcpClient.Port = 60000;
		}

		/// <summary>
		/// Connect to the device.
		/// </summary>
		[PublicAPI]
		public void Connect()
		{
			if (m_Port == null)
			{
				Log(eSeverity.Critical, "Unable to connect, port is null");
				return;
			}

			m_Port.Connect();
			IsConnected = m_Port.IsConnected;
		}

		/// <summary>
		/// Disconnect from the device.
		/// </summary>
		[PublicAPI]
		public void Disconnect()
		{
			if (m_Port == null)
			{
				Log(eSeverity.Critical, "Unable to disconnect, port is null");
				return;
			}

			m_Port.Disconnect();
			IsConnected = m_Port.IsConnected;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_Port.IsConnected;
		}

		/// <summary>
		/// Called periodically to maintain connection to the device.
		/// </summary>
		private void ConnectionTimerCallback()
		{
			if (m_Port != null && !m_Port.IsConnected)
				Connect();
		}

		/// <summary>
		/// Logs the message.
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		private void Log(eSeverity severity, string message, params object[] args)
		{
			message = string.Format(message, args);

			ServiceProvider.GetService<ILoggerService>().AddEntry(severity, AddLogPrefix(message));
		}

		/// <summary>
		/// Returns the log message with a LutronQuantumNwkDevice prefix.
		/// </summary>
		/// <param name="log"></param>
		/// <returns></returns>
		private string AddLogPrefix(string log)
		{
			return string.Format("{0} - {1}", GetType().Name, log);
		}

		#endregion

		#region Port Callbacks

		/// <summary>
		/// Subscribes to the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(ISerialPort port)
		{
			if (port == null)
				return;

			port.OnConnectedStateChanged += PortOnConnectedStateChanged;
			port.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribes from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(ISerialPort port)
		{
			if (port == null)
				return;

			port.OnConnectedStateChanged -= PortOnConnectedStateChanged;
			port.OnIsOnlineStateChanged -= PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Called when the port comes online or goes offline.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PortOnIsOnlineStateChanged(object sender, BoolEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Called when the port connects or disconnects.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PortOnConnectedStateChanged(object sender, BoolEventArgs args)
		{
			IsConnected = args.Data;

			if (IsConnected)
				Initialized = true;
			else
			{
				//m_SerialQueue.Clear();

				Log(eSeverity.Critical, "Lost connection");
				Initialized = false;

				m_ConnectionTimer.Reset(CONNECTION_CHECK_MILLISECONDS, CONNECTION_CHECK_MILLISECONDS);
			}
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetPort(null);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(RackLinkDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Port = m_Port == null ? (int?)null : m_Port.Id;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(RackLinkDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			ISerialPort port = null;

			if (settings.Port != null)
			{
				port = factory.GetPortById((int)settings.Port) as ISerialPort;
				if (port == null)
					IcdErrorLog.Error("No serial Port with id {0}", settings.Port);
			}

			SetPort(port);
		}

		#endregion
	}
}
