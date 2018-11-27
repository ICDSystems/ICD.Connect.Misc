using System;
using System.Collections.Generic;
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
			    {eComParityType.ComspecParityNone, SerialConfiguration.eParity.None},
			    {eComParityType.ComspecParityEven, SerialConfiguration.eParity.Even},
			    {eComParityType.ComspecParityOdd, SerialConfiguration.eParity.Odd}
		    };

		private readonly ComSpecProperties m_ComSpecProperties;

	    private readonly AsyncTcpClient m_Client;

		private IGcITachDevice m_Device;
		private int m_Module;
		private int m_Address;

		/// <summary>
		/// Gets the Com Spec configuration properties.
		/// </summary>
		protected override IComSpecProperties ComSpecProperties { get { return m_ComSpecProperties; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachComPort()
		{
			m_ComSpecProperties = new ComSpecProperties();

			m_Client = new AsyncTcpClient {Name = GetType().Name};
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
			SetModuleType(Module.eId.FlcSerial, Module.eClass.Serial, Module.eType.Rs232);

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
	    /// <param name="baudRate"></param>
	    /// <param name="numberOfDataBits"></param>
	    /// <param name="parityType"></param>
	    /// <param name="numberOfStopBits"></param>
	    /// <param name="protocolType"></param>
	    /// <param name="hardwareHandShake"></param>
	    /// <param name="softwareHandshake"></param>
	    /// <param name="reportCtsChanges"></param>
	    public override void SetComPortSpec(eComBaudRates baudRate, eComDataBits numberOfDataBits, eComParityType parityType,
											eComStopBits numberOfStopBits, eComProtocolType protocolType, eComHardwareHandshakeType hardwareHandShake,
											eComSoftwareHandshakeType softwareHandshake, bool reportCtsChanges)
		{
			GcITachFlexDevice flexDevice = m_Device as GcITachFlexDevice;
			if (flexDevice == null)
				return;

			string localUrl = string.Format("api/host/modules/{0}/ports/{1}/config", m_Module, m_Address);

			SerialConfiguration config = new SerialConfiguration
			{
				BaudRate = ComSpecUtils.BaudRateToRate(baudRate),
				Parity = s_ParityToSerialConfiguration.GetValue(parityType),
				StopBits = ComSpecUtils.StopBitsToCount(numberOfStopBits),
				FlowControl = hardwareHandShake == eComHardwareHandshakeType.ComspecHardwareHandshakeNone
					              ? SerialConfiguration.eFlowControl.None
					              : SerialConfiguration.eFlowControl.Hardware
			};

			try
			{
				flexDevice.Post(localUrl, config.Serialize());
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, e, "Failed to set comspec");
			}
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

	    /// <summary>
	    /// Sets the configuration for the module.
	    /// </summary>
	    /// <param name="id"></param>
	    /// <param name="class"></param>
	    /// <param name="type"></param>
	    private void SetModuleType(Module.eId id, Module.eClass @class, Module.eType type)
	    {
		    GcITachFlexDevice flexDevice = m_Device as GcITachFlexDevice;
		    if (flexDevice == null)
			    return;

		    Module module = new Module
		    {
			    Id = id,
			    Class = @class,
			    Type = type
		    };

		    string localUrl = string.Format("api/host/modules/{0}", m_Module);

		    try
		    {
			    flexDevice.Post(localUrl, module.Serialize());
		    }
		    catch (Exception e)
		    {
			    Log(eSeverity.Error, e, "Failed to set module type");
		    }
	    }

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

			m_ComSpecProperties.Clear();
		}

		protected override void CopySettingsFinal(GcITachComPortSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Module = m_Module;
			settings.Address = m_Address;
			settings.Device = m_Device == null ? (int?)null : m_Device.Id;

			settings.Copy(m_ComSpecProperties);
		}

		protected override void ApplySettingsFinal(GcITachComPortSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_ComSpecProperties.Copy(settings);

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
					Log(eSeverity.Error, "No device with id {0}", m_Device);
				}
			}

			SetDevice(device);
		}

		#endregion

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Device", m_Device);
			addRow("Module", m_Module);
			addRow("Address", m_Address);
		}

		#endregion
	}
}
