using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Misc.GlobalCache.FlexApi;
using ICD.Connect.Protocol;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Tcp;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.SerialBuffers;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	public sealed class GcITachFlexDevice : AbstractDevice<GcITachFlexDeviceSettings>
	{
		private const ushort TCP_PORT = 4998;

		private readonly ConnectionStateManager m_ConnectionStateManager;
		private readonly DelimiterSerialBuffer m_TcpBuffer;
		private readonly HttpPort m_HttpClient;

		/// <summary>
		/// Gets the network address of the device.
		/// </summary>
		public string Address
		{
			get
			{
				AsyncTcpClient client = m_ConnectionStateManager.Port as AsyncTcpClient;
				return client == null ? null : client.Address;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachFlexDevice()
		{
			m_TcpBuffer = new DelimiterSerialBuffer(FlexData.NEWLINE);

			m_ConnectionStateManager = new ConnectionStateManager(this) {ConfigurePort = ConfigurePort};
			m_ConnectionStateManager.OnConnectedStateChanged += PortOnConnectionStatusChanged;
			m_ConnectionStateManager.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
			m_ConnectionStateManager.OnSerialDataReceived += PortOnSerialDataReceived;

			m_HttpClient = new HttpPort
			{
				Name = GetType().Name
			};

			Subscribe(m_TcpBuffer);
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
			m_HttpClient.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the TCP client for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		public void SetPort(AsyncTcpClient port)
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

		/// <summary>
		/// Sends the data to the device.
		/// </summary>
		/// <param name="localUrl"></param>
		/// <param name="data"></param>
		public string Post(string localUrl, string data)
		{
			string result;
			m_HttpClient.Post(localUrl, data, out result);

			return result;
		}

		#endregion

		private void ConfigurePort(ISerialPort port)
		{
			AsyncTcpClient client = port as AsyncTcpClient;
			if (client != null)
				client.Port = TCP_PORT;
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			bool tcp = m_ConnectionStateManager.IsOnline;
			bool http = m_HttpClient != null && m_HttpClient.IsOnline;

			return tcp && http;
		}

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

			SetPort(null);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(GcITachFlexDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Port = m_ConnectionStateManager.PortNumber;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(GcITachFlexDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			AsyncTcpClient port = null;

			if (settings.Port != null)
			{
				try
				{
					port = factory.GetPortById((int)settings.Port) as AsyncTcpClient;
				}
				catch (KeyNotFoundException)
				{
					Log(eSeverity.Error, "No AsyncTcpClient with id {0}", settings.Port);
				}
			}

			SetPort(port);
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

			addRow("Address", Address);
		}

		#endregion
	}
}
