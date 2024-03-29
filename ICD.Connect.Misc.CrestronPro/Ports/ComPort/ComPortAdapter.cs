﻿using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Misc.CrestronPro.Extensions;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
#endif

namespace ICD.Connect.Misc.CrestronPro.Ports.ComPort
{
	/// <summary>
	/// ComPortWrapper wraps a SimplSharpPro ComPort.
	/// </summary>
	public sealed class ComPortAdapter : AbstractComPort<ComPortAdapterSettings>
	{
		private const eComBaudRates DEFAULT_BAUD_RATE = eComBaudRates.BaudRate9600;
		private const eComDataBits DEFAULT_DATA_BITS = eComDataBits.DataBits8;
		private const eComParityType DEFAULT_PARITY_TYPE = eComParityType.None;
		private const eComStopBits DEFAULT_STOP_BITS = eComStopBits.StopBits1;
		private const eComProtocolType DEFAULT_PROTOCOL_TYPE = eComProtocolType.Rs232;
		private const eComHardwareHandshakeType DEFAULT_HARDWARE_HANDSHAKE_TYPE = eComHardwareHandshakeType.None;
		private const eComSoftwareHandshakeType DEFAULT_SOFTWARE_HANDSHAKE_TYPE = eComSoftwareHandshakeType.None;
		private const bool DEFAULT_REPORT_CTS_CHANGES = false;

		private readonly ComSpecProperties m_ComSpecProperties;

#if !NETSTANDARD
		private Crestron.SimplSharpPro.ComPort m_Port;
#endif

		// Used with settings
		private int? m_Device;
		private int m_Address;

		#region Properties

		/// <summary>
		/// Gets the Com Spec configuration properties.
		/// </summary>
		public override IComSpecProperties ComSpecProperties { get { return m_ComSpecProperties; } }

		/// <summary>
		/// Gets the baud rate.
		/// </summary>
		public override eComBaudRates BaudRate
		{
			get
			{
#if !NETSTANDARD
				return m_Port == null ? DEFAULT_BAUD_RATE : m_Port.BaudRate.FromCrestron();
#else
				return DEFAULT_BAUD_RATE;
#endif
			}
		}

		/// <summary>
		/// Gets the number of data bits.
		/// </summary>
		public override eComDataBits NumberOfDataBits
		{
			get
			{
#if !NETSTANDARD
				return m_Port == null ? DEFAULT_DATA_BITS : m_Port.DataBits.FromCrestron();
#else
				return DEFAULT_DATA_BITS;
#endif
			}
		}

		/// <summary>
		/// Gets the parity type.
		/// </summary>
		public override eComParityType ParityType
		{
			get
			{
#if !NETSTANDARD
				return m_Port == null ? DEFAULT_PARITY_TYPE : m_Port.Parity.FromCrestron();
#else
				return DEFAULT_PARITY_TYPE;
#endif
			}
		}

		/// <summary>
		/// Gets the number of stop bits.
		/// </summary>
		public override eComStopBits NumberOfStopBits
		{
			get
			{
#if !NETSTANDARD
				return m_Port == null ? DEFAULT_STOP_BITS : m_Port.StopBits.FromCrestron();
#else
				return DEFAULT_STOP_BITS;
#endif
			}
		}

		/// <summary>
		/// Gets the protocol type.
		/// </summary>
		public override eComProtocolType ProtocolType
		{
			get
			{
#if !NETSTANDARD
				return m_Port == null ? DEFAULT_PROTOCOL_TYPE : m_Port.Protocol.FromCrestron();
#else
				return DEFAULT_PROTOCOL_TYPE;
#endif
			}
		}

		/// <summary>
		/// Gets the hardware handshake mode.
		/// </summary>
		public override eComHardwareHandshakeType HardwareHandshake
		{
			get
			{
#if !NETSTANDARD
				return m_Port == null ? DEFAULT_HARDWARE_HANDSHAKE_TYPE : m_Port.HwHandShake.FromCrestron();
#else
				return DEFAULT_HARDWARE_HANDSHAKE_TYPE;
#endif
			}
		}

		/// <summary>
		/// Gets the software handshake mode.
		/// </summary>
		public override eComSoftwareHandshakeType SoftwareHandshake
		{
			get
			{
#if !NETSTANDARD
				return m_Port == null ? DEFAULT_SOFTWARE_HANDSHAKE_TYPE : m_Port.SwHandShake.FromCrestron();
#else
				return DEFAULT_SOFTWARE_HANDSHAKE_TYPE;
#endif
			}
		}

		/// <summary>
		/// Gets the report CTS changes mode.
		/// </summary>
		public override bool ReportCtsChanges
		{
			get
			{
#if !NETSTANDARD
				return m_Port == null ? DEFAULT_REPORT_CTS_CHANGES : m_Port.ReportCTSChanges;
#else
				return DEFAULT_REPORT_CTS_CHANGES;
#endif
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ComPortAdapter()
		{
			m_ComSpecProperties = new ComSpecProperties();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

#if !NETSTANDARD
			// Unsbscribe and unregister
			SetComPort(null, 0);
#endif
		}

#if !NETSTANDARD
		/// <summary>
		/// Sets the com port.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="address"></param>
		[PublicAPI]
		public void SetComPort(Crestron.SimplSharpPro.ComPort port, int address)
		{
			m_Address = address;

			Unsubscribe(m_Port);
			Unregister(m_Port);

			m_Port = port;

			Register(m_Port);
			Subscribe(m_Port);

			UpdateCachedOnlineStatus();
			UpdateIsConnectedState();
		}

		/// <summary>
		/// Unregisters the given port.
		/// </summary>
		/// <param name="port"></param>
		private void Unregister(Crestron.SimplSharpPro.ComPort port)
		{
			if (port != null)
				PortDeviceUtils.Unregister(port);
		}

		/// <summary>
		/// Registers the port and then re-registers the parent.
		/// </summary>
		/// <param name="port"></param>
		private void Register(Crestron.SimplSharpPro.ComPort port)
		{
			try
			{
				if (port != null)
					PortDeviceUtils.Register(port);
			}
			catch (InvalidOperationException e)
			{
				Logger.Log(eSeverity.Error, "Error registering port - {0}", e.Message);
			}
		}
#endif

		/// <summary>
		/// Connects to the end point.
		/// </summary>
		public override void Connect()
		{
		}

		/// <summary>
		/// Disconnects from the end point.
		/// </summary>
		public override void Disconnect()
		{
		}

		/// <summary>
		/// Implements the actual sending logic. Wrapped by Send to handle connection status.
		/// </summary>
		protected override bool SendFinal(string data)
		{
#if !NETSTANDARD
			if (m_Port == null)
			{
				Logger.Log(eSeverity.Error, "Unable to send data - internal port is null");
				return false;
			}

			PrintTx(() => data);
			m_Port.Send(data);

			return true;
#else
			throw new NotSupportedException();
#endif
		}

		#endregion

		#region ComSpec

		/// <summary>
		/// Configures the ComPort for communication.
		/// </summary>
		/// <param name="comSpec"></param>
		[PublicAPI]
		public override void SetComPortSpec(ComSpec comSpec)
		{
#if !NETSTANDARD
			if (m_Port == null)
			{
				Logger.Log(eSeverity.Error, "Unable to set ComSpec - internal port is null");
				return;
			}

			m_Port.SetComPortSpec(comSpec.BaudRate.ToCrestron(),
			                      comSpec.NumberOfDataBits.ToCrestron(),
			                      comSpec.ParityType.ToCrestron(),
			                      comSpec.NumberOfStopBits.ToCrestron(),
			                      comSpec.ProtocolType.ToCrestron(),
			                      comSpec.HardwareHandshake.ToCrestron(),
			                      comSpec.SoftwareHandshake.ToCrestron(),
			                      comSpec.ReportCtsChanges);

#else
			throw new NotSupportedException();
#endif
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if !NETSTANDARD
			return m_Port != null && m_Port.GetIsRegisteredAndParentOnline();
#else
			return false;
#endif
		}

		/// <summary>
		/// Returns the connection state of the port
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsConnectedState()
		{
			return GetIsOnlineStatus();
		}

		#endregion

		#region Port Callbacks

#if !NETSTANDARD
		/// <summary>
		/// Subscribe to the port events.
		/// </summary>
		private void Subscribe(Crestron.SimplSharpPro.ComPort port)
		{
			if (port == null)
				return;

			port.SerialDataReceived += PortSerialDataReceived;

			GenericBase parent = port.Parent as GenericBase;
			if (parent != null)
				parent.OnlineStatusChange += ParentOnOnlineStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(Crestron.SimplSharpPro.ComPort port)
		{
			if (port == null)
				return;

			port.SerialDataReceived -= PortSerialDataReceived;

			GenericBase parent = port.Parent as GenericBase;
			if (parent != null)
				parent.OnlineStatusChange -= ParentOnOnlineStatusChange;
		}

		/// <summary>
		/// When we get some data from the serial port we add it to the buffer.
		/// </summary>
		/// <param name="receivingComPort"></param>
		/// <param name="args"></param>
		private void PortSerialDataReceived(Crestron.SimplSharpPro.ComPort receivingComPort, ComPortSerialDataEventArgs args)
		{
			PrintRx(() => args.SerialData);
			Receive(args.SerialData);
		}

		private void ParentOnOnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
			UpdateIsConnectedState();
		}
#endif

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(ComPortAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Device = m_Device;
			settings.Address = m_Address;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_Device = 0;

#if !NETSTANDARD
			SetComPort(null, 0);
#endif
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(ComPortAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

#if !NETSTANDARD
			m_Device = settings.Device;

			Crestron.SimplSharpPro.ComPort port = null;
			IPortParent provider = null;

			if (m_Device != null)
			{
				try
				{
					provider = factory.GetOriginatorById((int)m_Device) as IPortParent;
				}
				catch (KeyNotFoundException)
				{
					Logger.Log(eSeverity.Error, "No device with id {0}", m_Device);
				}
			}

			if (provider == null)
				Logger.Log(eSeverity.Error, "{0} is not a {1}", m_Device, typeof(IPortParent).Name);
			else
			{
				try
				{
					port = provider.GetComPort(settings.Address);
					if (port == null)
						Logger.Log(eSeverity.Error, "No Com Port at {0} address {1}", m_Device, settings.Address);
				}
				catch (Exception e)
				{
					Logger.Log(eSeverity.Error, "Unable to get ComPort from device {0} at address {1} - {2}", m_Device,
					           settings.Address, e.Message);
				}
			}

			if (provider != null && port == null)
				Logger.Log(eSeverity.Error, "No Com Port at {0} address {1}", m_Device, settings.Address);

			SetComPort(port, settings.Address);

			ApplyConfiguration();
#endif
		}

		#endregion

		#region Console Commands

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Device", m_Device);
			addRow("Address", m_Address);
		}

		#endregion
	}
}
