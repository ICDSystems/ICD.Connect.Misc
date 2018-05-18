using System;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Misc.GlobalCache.FlexApi;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.SerialBuffers;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	public sealed class GcITachFlexDevice : AbstractDevice<GcITachFlexDeviceSettings>
	{
		private readonly UriProperties m_UriProperties;
		private readonly SecureNetworkProperties m_NetworkProperties;
		private readonly DelimiterSerialBuffer m_Buffer;

		[CanBeNull] private ISerialPort m_SerialPort;
		[CanBeNull] private IWebPort m_WebPort;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachFlexDevice()
		{
			m_UriProperties = new UriProperties();
			m_NetworkProperties = new SecureNetworkProperties();

			m_Buffer = new DelimiterSerialBuffer(FlexData.NEWLINE);
			Subscribe(m_Buffer);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(m_Buffer);

			SetSerialPort(null);
			SetWebPort(null);
		}

		#region Methods

		/// <summary>
		/// Sets the WebPort for communication with the GlobalCache device web API.
		/// </summary>
		/// <param name="port"></param>
		public void SetWebPort(IWebPort port)
		{
			if (port == m_WebPort)
				return;

			ConfigurePort(port);

			Unsubscribe(m_WebPort);
			m_WebPort = port;
			Subscribe(m_WebPort);
		}

		/// <summary>
		/// Sets the serial port for communication with the 
		/// </summary>
		/// <param name="port"></param>
		public void SetSerialPort(ISerialPort port)
		{
			if (port == m_SerialPort)
				return;

			ConfigurePort(port);

			Unsubscribe(m_SerialPort);

			m_Buffer.Clear();
			m_SerialPort = port;

			Subscribe(m_SerialPort);
		}

		/// <summary>
		/// Configures the given port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		private void ConfigurePort(IPort port)
		{
			// URI
			if (port is IWebPort)
				(port as IWebPort).ApplyDeviceConfiguration(m_UriProperties);

			// Network (TCP, UDP, SSH)
			if (port is ISecureNetworkPort)
				(port as ISecureNetworkPort).ApplyDeviceConfiguration(m_NetworkProperties);
			else if (port is INetworkPort)
				(port as INetworkPort).ApplyDeviceConfiguration(m_NetworkProperties);
		}

		/// <summary>
		/// Sends the command to the device.
		/// </summary>
		/// <param name="command"></param>
		public void SendCommand(string command)
		{
			if (m_SerialPort == null)
				throw new InvalidOperationException("Wrapped serial port is null");

			m_SerialPort.Send(command);
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
			if (m_WebPort == null)
				throw new InvalidOperationException("Wrapped web port is null");

			string result;
			if (m_WebPort.Post(localUrl, data, Encoding.ASCII, out result))
				ParseResult(result);
		}

		/// <summary>
		/// Gets the network address for the device.
		/// </summary>
		/// <returns></returns>
		public string GetNetworkAddress()
		{
			if (m_WebPort != null && !string.IsNullOrEmpty(m_WebPort.UriProperties.UriHost))
				return m_WebPort.UriProperties.UriHost;

			INetworkPort networkPort = m_SerialPort as INetworkPort;
			return networkPort != null ? networkPort.Address : null;
		}

		#endregion

		/// <summary>
		/// HTTP response handler.
		/// </summary>
		/// <param name="result"></param>
		private void ParseResult(string result)
		{
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			bool serial = m_SerialPort != null && m_SerialPort.IsOnline;
			bool web = m_WebPort != null && m_WebPort.IsOnline;

			return serial && web;
		}

		#region Web Port Callbacks

		/// <summary>
		/// Subscribe to the web port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(IWebPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged += WebPortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the web port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(IWebPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged -= WebPortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Called when the web port online status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void WebPortOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs eventArgs)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Serial Port Callbacks

		/// <summary>
		/// Subscribe to the port callbacks.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(ISerialPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged += SerialPortOnIsOnlineStateChanged;
			port.OnSerialDataReceived += SerialPortOnSerialDataReceived;
		}

		/// <summary>
		/// Unsubscribe from the port callbacks.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(ISerialPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged -= SerialPortOnIsOnlineStateChanged;
			port.OnSerialDataReceived -= SerialPortOnSerialDataReceived;
		}

		/// <summary>
		/// Called when we receive data from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void SerialPortOnSerialDataReceived(object sender, StringEventArgs stringEventArgs)
		{
			m_Buffer.Enqueue(stringEventArgs.Data);
		}

		/// <summary>
		/// Called when the serial port online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SerialPortOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Serial Buffer Callbacks

		/// <summary>
		/// Subscribe to the buffer events.
		/// </summary>
		/// <param name="buffer"></param>
		private void Subscribe(ISerialBuffer buffer)
		{
			buffer.OnCompletedSerial += BufferOnOnCompletedSerial;
		}

		/// <summary>
		/// Unsubscribe from the buffer events.
		/// </summary>
		/// <param name="buffer"></param>
		private void Unsubscribe(ISerialBuffer buffer)
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

			SetWebPort(null);
			SetSerialPort(null);

			m_NetworkProperties.Clear();
			m_UriProperties.Clear();
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(GcITachFlexDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_NetworkProperties.Copy(settings);
			m_UriProperties.Copy(settings);

			// Web Port
			IWebPort webPort = null;

			if (settings.WebPort != null)
			{
				webPort = factory.GetPortById((int)settings.WebPort) as IWebPort;
				if (webPort == null)
					Log(eSeverity.Error, "No web Port with id {0}", settings.WebPort);
			}

			SetWebPort(webPort);

			// Serial Port
			ISerialPort serialPort = null;

			if (settings.SerialPort != null)
			{
				serialPort = factory.GetPortById((int)settings.SerialPort) as ISerialPort;
				if (serialPort == null)
					Log(eSeverity.Error, "No serial Port with id {0}", settings.SerialPort);
			}

			SetSerialPort(serialPort);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(GcITachFlexDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.WebPort = m_WebPort == null ? (int?)null : m_WebPort.Id;
			settings.SerialPort = m_SerialPort == null ? (int?)null : m_SerialPort.Id;

			settings.Copy(m_UriProperties);
			settings.Copy(m_NetworkProperties);
		}

		#endregion
	}
}
