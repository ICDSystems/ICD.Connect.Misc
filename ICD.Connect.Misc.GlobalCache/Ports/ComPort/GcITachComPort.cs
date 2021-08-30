#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using System;
using System.Collections.Generic;
using ICD.Common.Logging.LoggingContexts;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.GlobalCache.Devices;
using ICD.Connect.Misc.GlobalCache.Devices.ITachFlex;
using ICD.Connect.Misc.GlobalCache.FlexApi.RestApi;
using ICD.Connect.Protocol.Network.Ports.Tcp;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Protocol.Utils;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.GlobalCache.Ports.ComPort
{
	public sealed class GcITachComPort : AbstractComPort<GcITachComPortSettings>, IGcITachPort
	{
		private const ushort PORT = 4999;

		private static readonly BiDictionary<eComParityType, SerialConfiguration.eParity> s_ParityToSerialConfiguration =
			new BiDictionary<eComParityType, SerialConfiguration.eParity>
			{
				{eComParityType.None, SerialConfiguration.eParity.None},
				{eComParityType.Even, SerialConfiguration.eParity.Even},
				{eComParityType.Odd, SerialConfiguration.eParity.Odd}
			};

		private static readonly ComSpec s_DefaultComSpec = new ComSpec
		{
			BaudRate = eComBaudRates.BaudRate9600,
			NumberOfDataBits = eComDataBits.DataBits8,
			ParityType = eComParityType.None,
			NumberOfStopBits = eComStopBits.StopBits1,
			ProtocolType = eComProtocolType.Rs232,
			HardwareHandshake = eComHardwareHandshakeType.None,
			SoftwareHandshake = eComSoftwareHandshakeType.None,
			ReportCtsChanges = false,
		};

		private readonly ComSpecProperties m_ComSpecProperties;
		private readonly IcdTcpClient m_Client;
		private readonly ComSpec m_ComSpec;

		private IGcITachDevice m_Device;
		private int m_Module;
		private int m_Address;

		#region Properties

		/// <summary>
		/// Gets the Com Spec configuration properties.
		/// </summary>
		public override IComSpecProperties ComSpecProperties { get { return m_ComSpecProperties; } }

		/// <summary>
		/// Gets the baud rate.
		/// </summary>
		public override eComBaudRates BaudRate { get { return m_ComSpec.BaudRate; } }

		/// <summary>
		/// Gets the number of data bits.
		/// </summary>
		public override eComDataBits NumberOfDataBits { get { return m_ComSpec.NumberOfDataBits; } }

		/// <summary>
		/// Gets the parity type.
		/// </summary>
		public override eComParityType ParityType { get { return m_ComSpec.ParityType; } }

		/// <summary>
		/// Gets the number of stop bits.
		/// </summary>
		public override eComStopBits NumberOfStopBits { get { return m_ComSpec.NumberOfStopBits; } }

		/// <summary>
		/// Gets the protocol type.
		/// </summary>
		public override eComProtocolType ProtocolType { get { return m_ComSpec.ProtocolType; } }

		/// <summary>
		/// Gets the hardware handshake mode.
		/// </summary>
		public override eComHardwareHandshakeType HardwareHandshake { get { return m_ComSpec.HardwareHandshake; } }

		/// <summary>
		/// Gets the software handshake mode.
		/// </summary>
		public override eComSoftwareHandshakeType SoftwareHandshake { get { return m_ComSpec.SoftwareHandshake; } }

		/// <summary>
		/// Gets the report CTS changes mode.
		/// </summary>
		public override bool ReportCtsChanges { get { return false; } }

		public IGcITachDevice Device { get { return m_Device; } }

		public int Module { get { return m_Module; } }

		public int Address { get { return m_Address; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachComPort()
		{
			m_ComSpecProperties = new ComSpecProperties();
			m_ComSpec = s_DefaultComSpec.Copy();

			m_Client = new IcdTcpClient {Name = GetType().Name};
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

			// First make sure the device is in the correct configuration
			SetModuleType(FlexApi.RestApi.Module.eId.FlcSerial, FlexApi.RestApi.Module.eClass.Serial, FlexApi.RestApi.Module.eType.Rs232);

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

		/// <summary>
		/// Configures the com port with the given attributes.
		/// </summary>
		/// <param name="comSpec"></param>
		public override void SetComPortSpec(ComSpec comSpec)
		{
			GcITachFlexDevice flexDevice = m_Device as GcITachFlexDevice;
			if (flexDevice == null)
			{
				Logger.Log(eSeverity.Warning, "Setting ComSpec is unsupported - Parent device must be configured manually.");
				return;
			}

			string localUrl = string.Format("api/host/modules/{0}/ports/{1}/config", m_Module, m_Address);

			SerialConfiguration config = new SerialConfiguration
			{
				BaudRate = ComSpecUtils.BaudRateToRate(comSpec.BaudRate).ToString(),
				Parity = s_ParityToSerialConfiguration.GetValue(comSpec.ParityType),
				FlowControl = comSpec.HardwareHandshake == eComHardwareHandshakeType.None
					              ? SerialConfiguration.eFlowControl.None
					              : SerialConfiguration.eFlowControl.Hardware
			};

			string result;

			try
			{
				result = flexDevice.Post(localUrl, JsonConvert.SerializeObject(config));
			}
			catch (Exception e)
			{
				Logger.Log(eSeverity.Error, e, "Failed to set comspec");
				return;
			}

			SerialConfiguration current = JsonConvert.DeserializeObject<SerialConfiguration>(result);
			UpdateComSpec(current);
		}

		/// <summary>
		/// Sets the parent device.
		/// </summary>
		/// <param name="device"></param>
		[PublicAPI]
		public void SetDevice(IGcITachDevice device)
		{
			if (device == m_Device)
				return;

			Disconnect();

			m_Device = device;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the known comspec details.
		/// </summary>
		/// <param name="current"></param>
		private void UpdateComSpec(SerialConfiguration current)
		{
			if (current.BaudRate != null)
			{
				int rate = int.Parse(current.BaudRate);
				m_ComSpec.BaudRate = ComSpecUtils.BaudRateFromRate(rate);
			}

			m_ComSpec.ParityType = s_ParityToSerialConfiguration.GetKey(current.Parity);

			m_ComSpec.HardwareHandshake =
				current.FlowControl == SerialConfiguration.eFlowControl.None
					? eComHardwareHandshakeType.None
					: eComHardwareHandshakeType.RtsCts;
		}

		/// <summary>
		/// Sets the configuration for the module.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="class"></param>
		/// <param name="type"></param>
		private void SetModuleType(Module.eId id, Module.eClass @class, Module.eType type)
		{
			GcITachPortHelper.SetModuleType(this, id, @class, type);
		}

		protected override bool GetIsConnectedState()
		{
			return m_Client != null && m_Client.IsConnected;
		}

		protected override bool GetIsOnlineStatus()
		{
			return m_Client != null && m_Client.IsOnline;
		}

		#endregion

		#region TCP Client Callbacks

		/// <summary>
		/// Subscribe to the client events.
		/// </summary>
		/// <param name="client"></param>
		private void Subscribe(IcdTcpClient client)
		{
			client.OnIsOnlineStateChanged += ClientOnOnIsOnlineStateChanged;
			client.OnConnectedStateChanged += ClientOnOnConnectedStateChanged;
			client.OnSerialDataReceived += ClientOnOnSerialDataReceived;
		}

		/// <summary>
		/// Unsubscribe from the client events.
		/// </summary>
		/// <param name="client"></param>
		private void Unsubscribe(IcdTcpClient client)
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
		/// <param name="args"></param>
		private void ClientOnOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs args)
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

			m_ComSpecProperties.ClearComSpecProperties();
			m_ComSpec.Copy(s_DefaultComSpec);
		}

		protected override void CopySettingsFinal(GcITachComPortSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Module = m_Module;
			settings.Address = m_Address;
			settings.Device = m_Device == null ? (int?)null : m_Device.Id;
		}

		protected override void ApplySettingsFinal(GcITachComPortSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_Module = settings.Module;
			m_Address = settings.Address;

			IGcITachDevice device = null;

			if (settings.Device != null)
			{
				try
				{
					device = factory.GetDeviceById((int)settings.Device) as IGcITachDevice;
				}
				catch (KeyNotFoundException)
				{
					Logger.Log(eSeverity.Error, "No device with id {0}", m_Device);
				}
			}

			SetDevice(device);

			ApplyConfiguration();
		}

		#endregion

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			GcITachPortHelper.BuildConsoleStatus(this, addRow);
		}

		#endregion
	}
}
