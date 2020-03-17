using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Misc.GlobalCache.FlexApi;
using ICD.Connect.Protocol;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports;
using ICD.Connect.Protocol.Network.Ports.Tcp;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.SerialBuffers;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	public abstract class AbstractGcITachDevice<TSettings> : AbstractDevice<TSettings>, IGcITachDevice
		where TSettings : IGcITachDeviceSettings, new()
	{
		private readonly NetworkProperties m_NetworkProperties;

		private readonly ConnectionStateManager m_ConnectionStateManager;
		private readonly DelimiterSerialBuffer m_TcpBuffer;

		/// <summary>
		/// Gets the network address of the device.
		/// </summary>
		public string Address
		{
			get
			{
				IcdTcpClient client = m_ConnectionStateManager.Port as IcdTcpClient;
				return client == null ? null : client.Address;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractGcITachDevice()
		{
			m_NetworkProperties = new NetworkProperties();

			m_TcpBuffer = new DelimiterSerialBuffer(FlexData.NEWLINE);
			Subscribe(m_TcpBuffer);

			m_ConnectionStateManager = new ConnectionStateManager(this) {ConfigurePort = ConfigurePort};
			m_ConnectionStateManager.OnConnectedStateChanged += PortOnConnectionStatusChanged;
			m_ConnectionStateManager.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
			m_ConnectionStateManager.OnSerialDataReceived += PortOnSerialDataReceived;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(m_TcpBuffer);

			m_ConnectionStateManager.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the TCP client for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		public void SetPort(IcdTcpClient port)
		{
			m_ConnectionStateManager.SetPort(port);
		}

		/// <summary>
		/// Sends the command to the device.
		/// </summary>
		/// <param name="command"></param>
		public void SendCommand(string command)
		{
			m_ConnectionStateManager.Send(command);
		}

		/// <summary>
		/// Sends the command to the device.
		/// </summary>
		/// <param name="command"></param>
		public void SendCommand(FlexData command)
		{
			SendCommand(command.Serialize());
		}

		#endregion

		#region Private Methods

		private void ConfigurePort(ISerialPort port)
		{
			// TCP
			if (port is INetworkPort)
				(port as INetworkPort).ApplyDeviceConfiguration(m_NetworkProperties);
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_ConnectionStateManager.IsOnline;
		}

		#endregion

		#region TCP Client Callbacks

		/// <summary>
		/// Called when we receive data from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void PortOnSerialDataReceived(object sender, StringEventArgs stringEventArgs)
		{
			m_TcpBuffer.Enqueue(stringEventArgs.Data);
		}

		/// <summary>
		/// Called when the port connection status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PortOnConnectionStatusChanged(object sender, BoolEventArgs args)
		{
		}

		/// <summary>
		/// Called when the port online status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PortOnIsOnlineStateChanged(object sender, BoolEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Buffer Callbacks

		/// <summary>
		/// Subsribe to the buffer events.
		/// </summary>
		/// <param name="buffer"></param>
		private void Subscribe(DelimiterSerialBuffer buffer)
		{
			buffer.OnCompletedSerial += BufferOnOnCompletedSerial;
		}

		/// <summary>
		/// Unsubscribe from the buffer events.
		/// </summary>
		/// <param name="buffer"></param>
		private void Unsubscribe(DelimiterSerialBuffer buffer)
		{
			buffer.OnCompletedSerial -= BufferOnOnCompletedSerial;
		}

		/// <summary>
		/// Called when we receive a complete message from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void BufferOnOnCompletedSerial(object sender, StringEventArgs stringEventArgs)
		{
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_NetworkProperties.ClearNetworkProperties();

			SetPort(null);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Port = m_ConnectionStateManager.PortNumber;

			settings.Copy(m_NetworkProperties);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_NetworkProperties.Copy(settings);

			IcdTcpClient port = null;

			if (settings.Port != null)
			{
				try
				{
					port = factory.GetPortById((int)settings.Port) as IcdTcpClient;
				}
				catch (KeyNotFoundException)
				{
					Log(eSeverity.Error, "No {0} with id {1}", typeof(IcdTcpClient), settings.Port);
				}
			}

			SetPort(port);
		}

		#endregion

		#region Console 
		
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			if (m_ConnectionStateManager != null)
				yield return m_ConnectionStateManager.Port;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}
		#endregion

	}
}
