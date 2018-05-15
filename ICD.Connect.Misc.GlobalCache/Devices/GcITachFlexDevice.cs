using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Misc.GlobalCache.FlexApi;
using ICD.Connect.Protocol.Network.Tcp;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Protocol.SerialBuffers;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	public sealed class GcITachFlexDevice : AbstractDevice<GcITachFlexDeviceSettings>
	{
		private const ushort TCP_PORT = 4998;

		private readonly AsyncTcpClient m_TcpClient;
		private readonly DelimiterSerialBuffer m_TcpBuffer;
		private readonly HttpPort m_HttpClient;

		public string Address { get { return m_TcpClient.Address; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachFlexDevice()
		{
			m_TcpClient = new AsyncTcpClient
			{
				Name = GetType().Name,
				Port = TCP_PORT
			};

			m_TcpBuffer = new DelimiterSerialBuffer(FlexData.NEWLINE);

			m_HttpClient = new HttpPort
			{
				Name = GetType().Name
			};

			Subscribe(m_TcpBuffer);
			Subscribe(m_TcpClient);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(m_TcpBuffer);
			Unsubscribe(m_TcpClient);

			m_TcpClient.Dispose();
			m_HttpClient.Dispose();
		}

		/// <summary>
		/// Sends the command to the device.
		/// </summary>
		/// <param name="command"></param>
		public void SendCommand(string command)
		{
			m_TcpClient.Send(command);
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
		public void Post(string localUrl, string data)
		{
			string result;
			if (m_HttpClient.Post(localUrl, data, out result))
				ParseResult(result);
		}

		/// <summary>
		/// HTTP response handler.
		/// </summary>
		/// <param name="result"></param>
		private void ParseResult(string result)
		{
		}

		protected override bool GetIsOnlineStatus()
		{
			bool tcp = m_TcpClient != null && m_TcpClient.IsOnline;
			bool http = m_HttpClient != null && m_HttpClient.IsOnline;

			return tcp && http;
		}

		#region TCP Client Callbacks

		/// <summary>
		/// Subscribe to the client callbacks.
		/// </summary>
		/// <param name="client"></param>
		private void Subscribe(AsyncTcpClient client)
		{
			client.OnIsOnlineStateChanged += ClientOnOnIsOnlineStateChanged;
			client.OnSerialDataReceived += ClientOnOnSerialDataReceived;
		}

		/// <summary>
		/// Unsubscribe from the client callbacks.
		/// </summary>
		/// <param name="client"></param>
		private void Unsubscribe(AsyncTcpClient client)
		{
			client.OnIsOnlineStateChanged -= ClientOnOnIsOnlineStateChanged;
			client.OnSerialDataReceived -= ClientOnOnSerialDataReceived;
		}

		/// <summary>
		/// Called when we receive data from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void ClientOnOnSerialDataReceived(object sender, StringEventArgs stringEventArgs)
		{
			m_TcpBuffer.Enqueue(stringEventArgs.Data);
		}

		/// <summary>
		/// Called when the 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ClientOnOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region TCP Buffer Callbacks

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
			IcdConsole.PrintLine(stringEventArgs.Data);
		}

		#endregion

		#region Settings

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_TcpClient.Address = null;
		}

		protected override void ApplySettingsFinal(GcITachFlexDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_TcpClient.Address = settings.Address;
			m_HttpClient.Address = settings.Address;
		}

		protected override void CopySettingsFinal(GcITachFlexDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Address = m_TcpClient.Address;
		}

		#endregion

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Address", Address);
		}

		#endregion
	}
}
