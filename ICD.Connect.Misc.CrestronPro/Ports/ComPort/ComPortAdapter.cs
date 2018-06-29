using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Settings.Core;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using ICD.Connect.Misc.CrestronPro.Utils.Extensions;
#endif

namespace ICD.Connect.Misc.CrestronPro.Ports.ComPort
{
	/// <summary>
	/// ComPortWrapper wraps a SimplSharpPro ComPort.
	/// </summary>
	public sealed class ComPortAdapter : AbstractSerialPort<ComPortAdapterSettings>, IComPort
	{
#if SIMPLSHARP
		private Crestron.SimplSharpPro.ComPort m_Port;
#endif

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

#if SIMPLSHARP
			// Unsbscribe and unregister
			SetComPort(null, 0);
#endif
		}

#if SIMPLSHARP
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
		}

		/// <summary>
		/// Unregisters the given port.
		/// </summary>
		/// <param name="port"></param>
		private void Unregister(Crestron.SimplSharpPro.ComPort port)
		{
			if (port == null || !port.Registered)
				return;

			port.UnRegister();
		}

		/// <summary>
		/// Registers the port and then re-registers the parent.
		/// </summary>
		/// <param name="port"></param>
		private void Register(Crestron.SimplSharpPro.ComPort port)
		{
			if (port == null || port.Registered)
				return;

			eDeviceRegistrationUnRegistrationResponse result = port.Register();
			if (result != eDeviceRegistrationUnRegistrationResponse.Success)
			{
				Log(eSeverity.Error, "Unable to register {0} - {1}", port.GetType().Name, result);
				return;
			}

			GenericDevice parent = port.Parent as GenericDevice;
			if (parent == null)
				return;

			eDeviceRegistrationUnRegistrationResponse parentResult = parent.ReRegister();
			if (parentResult != eDeviceRegistrationUnRegistrationResponse.Success)
				Log(eSeverity.Error, "Unable to register parent {0} - {1}", parent.GetType().Name, parentResult);
		}
#endif

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
#if SIMPLSHARP
			if (m_Port == null)
			{
				Log(eSeverity.Error, "Unable to send - internal port is null");
				return false;
			}

			PrintTx(data);
			m_Port.Send(data);

			return true;
#else
            throw new NotSupportedException();
#endif
		}

		#endregion

		#region ComSpec

		[PublicAPI]
		public void SetComPortSpec(eComBaudRates baudRate, eComDataBits numberOfDataBits,
		                           eComParityType parityType,
		                           eComStopBits numberOfStopBits, eComProtocolType protocolType,
		                           eComHardwareHandshakeType hardwareHandShake,
		                           eComSoftwareHandshakeType softwareHandshake, bool reportCtsChanges)
		{
#if SIMPLSHARP
			if (m_Port == null)
			{
				Log(eSeverity.Error, "Unable to set ComSpec - internal port is null");
				return;
			}

			m_Port.SetComPortSpec((Crestron.SimplSharpPro.ComPort.eComBaudRates)(int)baudRate,
			                      (Crestron.SimplSharpPro.ComPort.eComDataBits)(int)numberOfDataBits,
			                      ParseEnum<Crestron.SimplSharpPro.ComPort.eComParityType>(parityType),
			                      (Crestron.SimplSharpPro.ComPort.eComStopBits)(int)numberOfStopBits,
			                      ParseEnum<Crestron.SimplSharpPro.ComPort.eComProtocolType>(protocolType),
			                      ParseEnum<Crestron.SimplSharpPro.ComPort.eComHardwareHandshakeType>(hardwareHandShake),
			                      ParseEnum<Crestron.SimplSharpPro.ComPort.eComSoftwareHandshakeType>(softwareHandshake),
			                      reportCtsChanges);

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
#if SIMPLSHARP
			return m_Port != null;
#else
            return false;
#endif
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

#if SIMPLSHARP
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

#if SIMPLSHARP
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

#if SIMPLSHARP
			m_Device = settings.Device;

			Crestron.SimplSharpPro.ComPort port = null;
			IPortParent provider = null;

			if (m_Device != null)
				provider = factory.GetDeviceById((int)m_Device) as IPortParent;

			if (provider == null)
				Log(eSeverity.Error, "{0} is not a {1}", m_Device, typeof(IPortParent).Name);
			else
			{
				try
				{
					port = provider.GetComPort(settings.Address);
				}
				catch (Exception e)
				{
					Log(eSeverity.Error, "Unable to get ComPort from device {0} at address {1} - {2}", m_Device,
					    settings.Address, e.Message);
				}
			}

			if (provider != null && port == null)
				Log(eSeverity.Error, "No Com Port at {0} address {1}", m_Device, settings.Address);

			SetComPort(port, settings.Address);
#endif
		}

		#endregion

		#region Console Commands

#if SIMPLSHARP
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
#endif

		#endregion
	}
}
