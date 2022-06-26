using System;
using ICD.Common.Properties;
using ICD.Connect.API.Nodes;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Misc.CrestronPro.Extensions;
using ICD.Connect.Misc.CrestronPro.Utils;
using System.Collections.Generic;
using System.Text;
#endif

namespace ICD.Connect.Misc.CrestronPro.Ports.IrOneWayComPort
{
	public sealed class IrOneWayComPortAdapter : AbstractComPort<IrOneWayComPortAdapterSettings>
	{
	    #region Defaults

	    private const eComBaudRates DEFAULT_BAUD_RATE = eComBaudRates.BaudRate9600;
		private const eComDataBits DEFAULT_DATA_BITS = eComDataBits.DataBits8;
		private const eComParityType DEFAULT_PARITY_TYPE = eComParityType.None;
		private const eComStopBits DEFAULT_STOP_BITS = eComStopBits.StopBits1;
		private const eComProtocolType DEFAULT_PROTOCOL_TYPE = eComProtocolType.Rs232;
		private const eComHardwareHandshakeType DEFAULT_HARDWARE_HANDSHAKE_TYPE = eComHardwareHandshakeType.None;
		private const eComSoftwareHandshakeType DEFAULT_SOFTWARE_HANDSHAKE_TYPE = eComSoftwareHandshakeType.None;
		private const bool DEFAULT_REPORT_CTS_CHANGES = false;

	    #endregion

	    #region Fields

	    private readonly ComSpecProperties m_ComSpecProperties;

	    private int? m_Device;

	    private int m_Address;

#if !NETSTANDARD

		private IROutputPort m_Port;

#endif

        #endregion

        #region Constructor

        public IrOneWayComPortAdapter()
	    {
	        m_ComSpecProperties = new ComSpecProperties();
	    }

	    #endregion

#if !NETSTANDARD
	    private void SetIrPort(IROutputPort port)
	    {
	        Unsubscribe(m_Port);
            Unregister(m_Port);

	        m_Port = port;

            Register(m_Port);
            Subscribe(m_Port);

            UpdateCachedOnlineStatus();
            UpdateIsConnectedState();
	    }

	    private void Register(IROutputPort port)
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

	    private void Unregister(IROutputPort port)
	    {
	        if (port != null)
                PortDeviceUtils.Unregister(port);
	    }

		/// <summary>
		/// Subscribe to the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(IROutputPort port)
		{
			if (port == null)
				return;

			GenericBase parent = port.Parent as GenericBase;
			if (parent != null)
				parent.OnlineStatusChange += ParentOnOnlineStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(IROutputPort port)
		{
			if (port == null)
				return;

			GenericBase parent = port.Parent as GenericBase;
			if (parent != null)
				parent.OnlineStatusChange -= ParentOnOnlineStatusChange;
		}

		private void ParentOnOnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
            UpdateIsConnectedState();
		}

#endif

        		#region Private Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if !NETSTANDARD
            return m_Port != null && m_Port.GetParentOnline();
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

	    #region IComPort

	    /// <summary>
		/// Sends the data to the remote endpoint.
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
			m_Port.SendSerialData(data);

			return true;
#else
			throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Gets the Com Spec configuration properties.
		/// </summary>
		public override IComSpecProperties ComSpecProperties
		{
			get { return m_ComSpecProperties; }
		}

		/// <summary>
		/// Gets the baud rate.
		/// </summary>
		public override eComBaudRates BaudRate
		{
			get { return ComSpecProperties.ComSpecBaudRate ?? DEFAULT_BAUD_RATE; }
		}

		/// <summary>
		/// Gets the number of data bits.
		/// </summary>
		public override eComDataBits NumberOfDataBits
		{
			get { return ComSpecProperties.ComSpecNumberOfDataBits ?? DEFAULT_DATA_BITS; }
		}

		/// <summary>
		/// Gets the parity type.
		/// </summary>
		public override eComParityType ParityType
		{
			get { return ComSpecProperties.ComSpecParityType ?? DEFAULT_PARITY_TYPE; }
		}

		/// <summary>
		/// Gets the number of stop bits.
		/// </summary>
		public override eComStopBits NumberOfStopBits
		{
			get { return ComSpecProperties.ComSpecNumberOfStopBits ?? DEFAULT_STOP_BITS; }
		}

		/// <summary>
		/// Gets the protocol type.
		/// </summary>
		public override eComProtocolType ProtocolType
		{
			get { return DEFAULT_PROTOCOL_TYPE; }
		}

		/// <summary>
		/// Gets the hardware handshake mode.
		/// </summary>
		public override eComHardwareHandshakeType HardwareHandshake
		{
			get { return DEFAULT_HARDWARE_HANDSHAKE_TYPE; }
		}

		/// <summary>
		/// Gets the software handshake mode.
		/// </summary>
		public override eComSoftwareHandshakeType SoftwareHandshake
		{
			
			get { return DEFAULT_SOFTWARE_HANDSHAKE_TYPE; }
		}

		/// <summary>
		/// Gets the report CTS changes mode.
		/// </summary>
		public override bool ReportCtsChanges
		{
			get { return DEFAULT_REPORT_CTS_CHANGES; }
		}

		/// <summary>
		/// Configures the ComPort for communication.
		/// </summary>
		/// <param name="comSpec"></param>
		public override void SetComPortSpec([NotNull] ComSpec comSpec)
		{
#if !NETSTANDARD
			if (comSpec == null)
				throw new ArgumentNullException("comSpec");

			if (m_Port == null)
			{
				Logger.Log(eSeverity.Error, "Unable to set com spec - internal port is null");
				return;
			}

			// Check for unsupported com spec options
			if (comSpec.ProtocolType != DEFAULT_PROTOCOL_TYPE)
				throw new NotSupportedException(string.Format("IrOneWayComPort does not support protocol {0}", comSpec.ProtocolType));
			if (comSpec.HardwareHandshake != DEFAULT_HARDWARE_HANDSHAKE_TYPE)
				throw new NotSupportedException(string.Format("IrOneWayComPort does not support hardware handshake type {0}", comSpec.HardwareHandshake));
			if (comSpec.SoftwareHandshake != DEFAULT_SOFTWARE_HANDSHAKE_TYPE)
				throw new NotSupportedException(string.Format("IrOneWayComPort does not support software handshake type {0}", comSpec.SoftwareHandshake));
			if (comSpec.ReportCtsChanges != DEFAULT_REPORT_CTS_CHANGES)
				throw new NotSupportedException(string.Format("IrOneWayComPort does not support report cts changes {0}", comSpec.ReportCtsChanges));

			eIRSerialBaudRates baudRate;
			try
			{
				baudRate = comSpec.BaudRate.ToCrestronIr();
			}
			catch (KeyNotFoundException)
			{
				throw new NotSupportedException(string.Format("IrOneWayComPort does not support baud rate {0}", comSpec.BaudRate));
			}

			eIRSerialDataBits dataBits = comSpec.NumberOfDataBits.ToCrestronIr();
			eIRSerialStopBits stopBits = comSpec.NumberOfStopBits.ToCrestronIr();

			eIRSerialParityType parity;
			try
			{
				parity = comSpec.ParityType.ToCrestronIr();
			}
			catch (KeyNotFoundException)
			{
				throw new NotSupportedException(string.Format("IrOneWayComPort does not support parity type {0}", comSpec.ParityType));
			}

			m_Port.SetIRSerialSpec(baudRate, dataBits, parity, stopBits, Encoding.Unicode);
            
            ComSpecProperties.Copy(comSpec);

#else
			throw new NotSupportedException();
#endif
		}

	    #endregion

	    #region Settings

	    /// <summary>
	    /// Override to clear the instance settings.
	    /// </summary>
	    protected override void ClearSettingsFinal()
	    {
	        base.ClearSettingsFinal();

	        m_Device = 0;
	        m_Address = 0;

#if !NETSTANDARD
			SetIrPort(null);
#endif
	    }

	    /// <summary>
	    /// Override to apply properties to the settings instance.
	    /// </summary>
	    /// <param name="settings"></param>
	    protected override void CopySettingsFinal(IrOneWayComPortAdapterSettings settings)
	    {
	        base.CopySettingsFinal(settings);

	        settings.Device = m_Device;
	        settings.Address = m_Address;
	    }

	    /// <summary>
	    /// Override to apply settings to the instance.
	    /// </summary>
	    /// <param name="settings"></param>
	    /// <param name="factory"></param>
	    protected override void ApplySettingsFinal(IrOneWayComPortAdapterSettings settings, IDeviceFactory factory)
	    {
	        base.ApplySettingsFinal(settings, factory);

	        m_Address = settings.Address;
	        m_Device = settings.Device;

#if !NETSTANDARD
            IROutputPort port = null;
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
                Logger.Log(eSeverity.Error, "{0} is not a port provider", m_Device);
            else
            {
                try
                {
                    port = provider.GetIrOutputPort(settings.Address);
                }
                catch (Exception e)
                {
                    Logger.Log(eSeverity.Error, "Unable to get IrPort from device {0} at address {1} - {2}", m_Device,
                        settings.Address, e.Message);
                }
            }

            if (provider != null && port == null)
                Logger.Log(eSeverity.Error, "No IR Port at {0} address {1}", m_Device, settings.Address);

            SetIrPort(port);

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