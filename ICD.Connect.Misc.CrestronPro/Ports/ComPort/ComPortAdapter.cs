using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Protocol.Utils;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Ports.ComPort
{
	/// <summary>
	/// ComPortWrapper wraps a SimplSharpPro ComPort.
	/// </summary>
	public sealed class ComPortAdapter : AbstractSerialPort<ComPortAdapterSettings>, IComPort
	{
		private Crestron.SimplSharpPro.ComPort m_Port;

		// Used with settings
		private int? m_Device;
		private int m_Address;

		/// <summary>
		/// Returns the connection state of the port.
		/// </summary>
		public override bool IsConnected { get { return true; } protected set { } }

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			// Unsbscribe and unregister
			SetComPort(null, 0);
		}

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

			if (m_Port != null && m_Port.Registered)
				m_Port.UnRegister();

			m_Port = port;
			if (m_Port != null && !m_Port.Registered)
			{
				eDeviceRegistrationUnRegistrationResponse result = m_Port.Register();
				if (result != eDeviceRegistrationUnRegistrationResponse.Success)
					Logger.AddEntry(eSeverity.Error, "Unable to register {0} - {1}", m_Port.GetType().Name, result);
			}

			Subscribe(m_Port);

			UpdateCachedOnlineStatus();
		}

		public override void Connect()
		{
			IsConnected = true;
		}

		public override void Disconnect()
		{
			IsConnected = false;
		}

		/// <summary>
		/// Returns the connection state of the port
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsConnectedState()
		{
			return IsConnected;
		}

		protected override bool SendFinal(string data)
		{
			if (m_Port == null)
			{
				string message = string.Format("{0} internal port is null", this);
				throw new InvalidOperationException(message);
			}

			PrintTx(data);
			m_Port.Send(data);

			return true;
		}

		#endregion

		#region ComSpec

		[PublicAPI]
		public int SetComPortSpec(eComBaudRates baudRate, eComDataBits numberOfDataBits,
		                          eComParityType parityType,
		                          eComStopBits numberOfStopBits, eComProtocolType protocolType,
		                          eComHardwareHandshakeType hardwareHandShake,
		                          eComSoftwareHandshakeType softwareHandshake, bool reportCtsChanges)
		{
			SetBaudRate(baudRate);
			SetDataBits(numberOfDataBits);
			SetParityType(parityType);
			SetStopBits(numberOfStopBits);
			SetProtocolType(protocolType);
			SetHardwardHandshake(hardwareHandShake);
			SetSoftwareHandshake(softwareHandshake);

			// Only care about the final value
			return SetReportCtsChanges(reportCtsChanges);
		}

		[PublicAPI]
		public int SetBaudRate(eComBaudRates baudRate)
		{
			if (m_Port == null)
			{
				string message = string.Format("{0} internal port is null", this);
				throw new InvalidOperationException(message);
			}

			// Cast to int since the numeric values are tied to real world values.
			int proBaudRate = (int)baudRate;

			return m_Port.SetComPortSpec((Crestron.SimplSharpPro.ComPort.eComBaudRates)proBaudRate,
			                             m_Port.DataBits,
			                             m_Port.Parity,
			                             m_Port.StopBits,
			                             m_Port.Protocol,
			                             m_Port.HwHandShake,
			                             m_Port.SwHandShake,
			                             m_Port.ReportCTSChanges);
		}

		[PublicAPI]
		public int SetDataBits(eComDataBits numberOfDataBits)
		{
			if (m_Port == null)
			{
				string message = string.Format("{0} internal port is null", this);
				throw new InvalidOperationException(message);
			}

			// Cast to int since the numeric values are tied to real world values.
			int proDataBits = (int)numberOfDataBits;

			return m_Port.SetComPortSpec(m_Port.BaudRate,
			                             (Crestron.SimplSharpPro.ComPort.eComDataBits)proDataBits,
			                             m_Port.Parity,
			                             m_Port.StopBits,
			                             m_Port.Protocol,
			                             m_Port.HwHandShake,
			                             m_Port.SwHandShake,
			                             m_Port.ReportCTSChanges);
		}

		[PublicAPI]
		public int SetParityType(eComParityType parityType)
		{
			if (m_Port == null)
			{
				string message = string.Format("{0} internal port is null", this);
				throw new InvalidOperationException(message);
			}

			// This is messier than casting to int, but it should be safer if the ComPort enum definitions ever change.
			Crestron.SimplSharpPro.ComPort.eComParityType proParityType =
				ParseEnum<Crestron.SimplSharpPro.ComPort.eComParityType>(parityType);

			return m_Port.SetComPortSpec(m_Port.BaudRate,
			                             m_Port.DataBits,
			                             proParityType,
			                             m_Port.StopBits,
			                             m_Port.Protocol,
			                             m_Port.HwHandShake,
			                             m_Port.SwHandShake,
			                             m_Port.ReportCTSChanges);
		}

		[PublicAPI]
		public int SetStopBits(eComStopBits numberOfStopBits)
		{
			if (m_Port == null)
			{
				string message = string.Format("{0} internal port is null", this);
				throw new InvalidOperationException(message);
			}

			// Cast to int since the numeric values are tied to real world values.
			int proStopBits = (int)numberOfStopBits;

			return m_Port.SetComPortSpec(m_Port.BaudRate,
			                             m_Port.DataBits,
			                             m_Port.Parity,
			                             (Crestron.SimplSharpPro.ComPort.eComStopBits)proStopBits,
			                             m_Port.Protocol,
			                             m_Port.HwHandShake,
			                             m_Port.SwHandShake,
			                             m_Port.ReportCTSChanges);
		}

		[PublicAPI]
		public int SetProtocolType(eComProtocolType protocolType)
		{
			if (m_Port == null)
			{
				string message = string.Format("{0} internal port is null", this);
				throw new InvalidOperationException(message);
			}

			// This is messier than casting to int, but it should be safer if the ComPort enum definitions ever change.
			Crestron.SimplSharpPro.ComPort.eComProtocolType proProtocolType =
				ParseEnum<Crestron.SimplSharpPro.ComPort.eComProtocolType>(protocolType);

			return m_Port.SetComPortSpec(m_Port.BaudRate,
			                             m_Port.DataBits,
			                             m_Port.Parity,
			                             m_Port.StopBits,
			                             proProtocolType,
			                             m_Port.HwHandShake,
			                             m_Port.SwHandShake,
			                             m_Port.ReportCTSChanges);
		}

		[PublicAPI]
		public int SetHardwardHandshake(eComHardwareHandshakeType hardwareHandShake)
		{
			if (m_Port == null)
			{
				string message = string.Format("{0} internal port is null", this);
				throw new InvalidOperationException(message);
			}

			// This is messier than casting to int, but it should be safer if the ComPort enum definitions ever change.
			Crestron.SimplSharpPro.ComPort.eComHardwareHandshakeType proHardwareHandshake =
				ParseEnum<Crestron.SimplSharpPro.ComPort.eComHardwareHandshakeType>(hardwareHandShake);

			return m_Port.SetComPortSpec(m_Port.BaudRate,
			                             m_Port.DataBits,
			                             m_Port.Parity,
			                             m_Port.StopBits,
			                             m_Port.Protocol,
			                             proHardwareHandshake,
			                             m_Port.SwHandShake,
			                             m_Port.ReportCTSChanges);
		}

		[PublicAPI]
		public int SetSoftwareHandshake(eComSoftwareHandshakeType softwareHandshake)
		{
			if (m_Port == null)
			{
				string message = string.Format("{0} internal port is null", this);
				throw new InvalidOperationException(message);
			}

			// This is messier than casting to int, but it should be safer if the ComPort enum definitions ever change.
			Crestron.SimplSharpPro.ComPort.eComSoftwareHandshakeType proSoftwareHandshake =
				ParseEnum<Crestron.SimplSharpPro.ComPort.eComSoftwareHandshakeType>(softwareHandshake);

			return m_Port.SetComPortSpec(m_Port.BaudRate,
			                             m_Port.DataBits,
			                             m_Port.Parity,
			                             m_Port.StopBits,
			                             m_Port.Protocol,
			                             m_Port.HwHandShake,
			                             proSoftwareHandshake,
			                             m_Port.ReportCTSChanges);
		}

		[PublicAPI]
		public int SetReportCtsChanges(bool reportCtsChanges)
		{
			if (m_Port == null)
			{
				string message = string.Format("{0} internal port is null", this);
				throw new InvalidOperationException(message);
			}

			return m_Port.SetComPortSpec(m_Port.BaudRate,
			                             m_Port.DataBits,
			                             m_Port.Parity,
			                             m_Port.StopBits,
			                             m_Port.Protocol,
			                             m_Port.HwHandShake,
			                             m_Port.SwHandShake,
			                             reportCtsChanges);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_Port.IsOnline;
		}

		/// <summary>
		/// Parses the input enum to the destination type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumValue"></param>
		/// <returns></returns>
		private static T ParseEnum<T>(object enumValue)
		{
			return EnumUtils.Parse<T>(enumValue.ToString(), true);
		}

		#endregion

		#region Port Callbacks

		/// <summary>
		/// Subscribe to the port events.
		/// </summary>
		private void Subscribe(Crestron.SimplSharpPro.ComPort port)
		{
			if (port == null)
				return;

			port.SerialDataReceived += PortSerialDataReceived;
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
		}

		/// <summary>
		/// When we get some data from the serial port we add it to the buffer.
		/// </summary>
		/// <param name="receivingComPort"></param>
		/// <param name="args"></param>
		private void PortSerialDataReceived(Crestron.SimplSharpPro.ComPort receivingComPort, ComPortSerialDataEventArgs args)
		{
			PrintRx(args.SerialData);
			Receive(args.SerialData);
		}

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
			SetComPort(null, 0);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(ComPortAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_Device = settings.Device;

			Crestron.SimplSharpPro.ComPort port = null;
			IPortParent provider = null;

			if (m_Device != null)
				provider = factory.GetDeviceById((int)m_Device) as IPortParent;

			if (provider == null)
				Logger.AddEntry(eSeverity.Error, "{0} is not a {1}", m_Device, typeof(IPortParent).Name);
			else
			{
				try
				{
					port = provider.GetComPort(settings.Address);
				}
				catch (Exception e)
				{
					Logger.AddEntry(eSeverity.Error, e, "Unable to get ComPort from device {0} at address {1}", m_Device,
					                settings.Address);
				}
			}

			if (provider != null && port == null)
				Logger.AddEntry(eSeverity.Error, "No Com Port at {0} address {1}", m_Device, settings.Address);

			SetComPort(port, settings.Address);
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

			addRow("Baud Rate", m_Port.BaudRate);
			addRow("Data Bits", m_Port.DataBits);
			addRow("Parity Type", m_Port.Parity);
			addRow("Stop Bits", m_Port.StopBits);
			addRow("Protocol Type", m_Port.Protocol);
			addRow("Hardware Handshake", m_Port.HwHandShake);
			addRow("Software Handshake", m_Port.SwHandShake);
			addRow("Report CTS Changes", m_Port.ReportCTSChanges);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<int, eComParityType, int, int>("Set232", "Set232 <baud> <parity> <data> <stop>", (a,b,c,d) => Set232Spec(a,b,c,d));

			yield return new GenericConsoleCommand<int>("SetBaudRate", BaudHelp(), v => SetBaudRate(ComSpecUtils.BaudRateFromRate(v)));
			yield return new GenericConsoleCommand<int>("SetDataBits", EnumValueHelp<eComDataBits>(), v => SetDataBits((eComDataBits)v));
			yield return new GenericConsoleCommand<eComParityType>("SetParity", EnumHelp<eComParityType>(), v => SetParityType(v));
			yield return new GenericConsoleCommand<int>("SetStopBits", EnumValueHelp<eComStopBits>(), v => SetStopBits((eComStopBits)v));
			yield return new GenericConsoleCommand<eComProtocolType>("SetProtocol", EnumHelp<eComProtocolType>(), v => SetProtocolType(v));
			yield return new GenericConsoleCommand<eComHardwareHandshakeType>("SetHwHandshake", EnumHelp<eComHardwareHandshakeType>(), v => SetHardwardHandshake(v));
			yield return new GenericConsoleCommand<eComSoftwareHandshakeType>("SetSwHandshake", EnumHelp<eComSoftwareHandshakeType>(), v => SetSoftwareHandshake(v));
			yield return new GenericConsoleCommand<bool>("ReportCts", "true/false", v => SetReportCtsChanges(v));
		}

		private void Set232Spec(int baudRate, eComParityType parity, int dataBits, int stopBits)
		{
			SetBaudRate(ComSpecUtils.BaudRateFromRate(baudRate));
			SetParityType(parity);
			SetDataBits((eComDataBits)dataBits);
			SetStopBits((eComStopBits)stopBits);
		}

		private string EnumValueHelp<T>()
		{
			return StringUtils.ArrayFormat(EnumUtils.GetValues<T>().Select(v => EnumUtils.GetUnderlyingValue(v)));
		}

		private string EnumHelp<T>()
		{
			return StringUtils.ArrayFormat(EnumUtils.GetValues<T>());
		}

		private string BaudHelp()
		{
			return StringUtils.ArrayFormat(EnumUtils.GetValues<eComBaudRates>().Select(v => ComSpecUtils.BaudRateToRate(v)).Order());
		}

		/// <summary>
		/// Workaround for unverifiable code warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
