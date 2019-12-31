using ICD.Common.Utils.EventArguments;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components;
using ICD.Connect.Misc.Vibe.Settings;
using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;
using ICD.Connect.Panels.Server;
using ICD.Connect.Protocol;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.SerialBuffers;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Mock.Destination;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard
{
	public class VibeBoard : AbstractPanelServerDevice<VibeBoardSettings>
	{
		public event EventHandler<BoolEventArgs> OnConnectedStateChanged;
		public event EventHandler<BoolEventArgs> OnInitializedChanged;

		private const char CLIENT_DELIMITER = (char)0xFF;

		private readonly ConnectionStateManager m_ConnectionStateManager;
		private readonly DelimiterSerialBuffer m_SerialBuffer;
		private readonly VibeComponentFactory m_ComponentFactory;
		private readonly VibeResponseHandler m_ResponseHandler;
		private readonly SecureNetworkProperties m_NetworkProperties;

		private bool m_Initialized;
		private bool m_IsConnected;

		#region Properties

		/// <summary>
		/// Device Initialized Status.
		/// </summary>
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

		/// <summary>
		/// Returns true when the codec is connected.
		/// </summary>
		public bool IsConnected
		{
			get { return m_IsConnected; }
			private set
			{
				if (value == m_IsConnected)
					return;

				m_IsConnected = value;

				OnConnectedStateChanged.Raise(this, new BoolEventArgs(m_IsConnected));
			}
		}

		public VibeComponentFactory Components { get { return m_ComponentFactory; } }

		internal VibeResponseHandler ResponseHandler
		{
			get { return m_ResponseHandler; }
		}

		#endregion

		public VibeBoard()
		{
			m_ResponseHandler = new VibeResponseHandler();
			m_ComponentFactory = new VibeComponentFactory(this);

			m_NetworkProperties = new SecureNetworkProperties();

			m_ConnectionStateManager = new ConnectionStateManager(this) {ConfigurePort = ConfigurePort};
			Subscribe(m_ConnectionStateManager);

			m_SerialBuffer = new DelimiterSerialBuffer(CLIENT_DELIMITER);
			Subscribe(m_SerialBuffer);

			MockRouteDestinationControl routingControl = new MockRouteDestinationControl(this, 0);
			routingControl.SetInputs(new[] {new ConnectorInfo(1, eConnectionType.Video | eConnectionType.Audio)});

			Controls.Add(routingControl);
			Controls.Add(new VibeBoardVolumeControl(this, Controls.Count));
			Controls.Add(new VibeBoardPowerControl(this, Controls.Count));
			Controls.Add(new VibeBoardAppControl(this, Controls.Count));
		}

		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(m_ConnectionStateManager);
			Unsubscribe(m_SerialBuffer);
		}

		public void SendCommand(VibeCommand command)
		{
			m_ConnectionStateManager.Send(command.Serialize());
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_ConnectionStateManager.IsOnline;
		}

		/// <summary>
		/// Sets the port for communicating with the device.
		/// </summary>
		/// <param name="port"></param>
		[PublicAPI]
		public void SetPort(ISerialPort port)
		{
			m_ConnectionStateManager.SetPort(port);
		}

		/// <summary>
		/// Configures the given port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		private void ConfigurePort(ISerialPort port)
		{
			// SSH
			if (port is ISecureNetworkPort)
				(port as ISecureNetworkPort).ApplyDeviceConfiguration(m_NetworkProperties);
			// TCP
			else if (port is INetworkPort)
				(port as INetworkPort).ApplyDeviceConfiguration(m_NetworkProperties);
		}

		#region Port Callbacks

		/// <summary>
		/// Subscribes to the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(ConnectionStateManager port)
		{
			port.OnSerialDataReceived += PortOnSerialDataReceived;
			port.OnConnectedStateChanged += PortOnConnectionStatusChanged;
			port.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(ConnectionStateManager port)
		{
			port.OnSerialDataReceived -= PortOnSerialDataReceived;
			port.OnConnectedStateChanged -= PortOnConnectionStatusChanged;
			port.OnIsOnlineStateChanged -= PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Called when serial data is recieved from the port.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PortOnSerialDataReceived(object sender, StringEventArgs args)
		{
			m_SerialBuffer.Enqueue(args.Data);
		}

		/// <summary>
		/// Called when the port connection status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PortOnConnectionStatusChanged(object sender, BoolEventArgs args)
		{
			m_SerialBuffer.Clear();
			IsConnected = m_ConnectionStateManager.IsConnected;

			if (args.Data)
				return;

			Log(eSeverity.Critical, "Lost connection");
			Initialized = false;
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
		/// Subscribes to the buffer events.
		/// </summary>
		/// <param name="buffer"></param>
		private void Subscribe(ISerialBuffer buffer)
		{
			buffer.OnCompletedSerial += SerialBufferCompletedSerial;
		}

		/// <summary>
		/// Unsubscribe from the buffer events.
		/// </summary>
		/// <param name="buffer"></param>
		private void Unsubscribe(ISerialBuffer buffer)
		{
			buffer.OnCompletedSerial -= SerialBufferCompletedSerial;
		}

		/// <summary>
		/// Called when the buffer completes a string.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SerialBufferCompletedSerial(object sender, StringEventArgs args)
		{
			Initialized = true;
			m_ResponseHandler.HandleResponse(args.Data);
		}

		#endregion

		#region Settings

		protected override void ApplySettingsFinal(VibeBoardSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_NetworkProperties.Copy(settings);

			ISerialPort port = null;

			if (settings.KrangPort != null)
			{
				try
				{
					port = factory.GetPortById(settings.KrangPort.Value) as ISerialPort;
				}
				catch (KeyNotFoundException)
				{
					Log(eSeverity.Error, "No serial port with id {0}", settings.KrangPort);
				}
			}

			SetPort(port);
		}

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();
			
			m_NetworkProperties.ClearNetworkProperties();

			SetPort(null);
		}

		protected override void CopySettingsFinal(VibeBoardSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.KrangPort = m_ConnectionStateManager.PortNumber;

			settings.Copy(m_NetworkProperties);
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public override string ConsoleHelp { get { return "Vibe Board touch display device"; } }

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Initialized", Initialized);
			addRow("IsConnected", IsConnected);
		}

		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			yield return Components;
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
