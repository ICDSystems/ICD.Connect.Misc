using System;
using System.Collections.Generic;
using ICD.Connect.Misc.CrestronPro.Utils.Extensions;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
#endif
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Ports.IoPort;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Ports.IoPort
{
	public sealed class IoPortAdapter : AbstractIoPort<IoPortAdapterSettings>
	{
#if SIMPLSHARP
        private static readonly Dictionary<eIoPortConfiguration, eVersiportConfiguration> s_ConfigMap =
			new Dictionary<eIoPortConfiguration, eVersiportConfiguration>
			{
				{eIoPortConfiguration.None, eVersiportConfiguration.NotSet},
				{eIoPortConfiguration.AnalogIn, eVersiportConfiguration.AnalogInput},
				{eIoPortConfiguration.DigitalIn, eVersiportConfiguration.DigitalInput},
				{eIoPortConfiguration.DigitalOut, eVersiportConfiguration.DigitalOutput}
			};

		private Versiport m_Port;
#endif

		// Used with settings
		private int? m_Device;
		private int m_Address;

#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
#if SIMPLSHARP
            // Unregister.
            SetIoPort(null, 0);
#endif

			base.DisposeFinal(disposing);
		}

#if SIMPLSHARP
        /// <summary>
        /// Sets the wrapped port instance.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="address"></param>
        [PublicAPI]
		public void SetIoPort(Versiport port, int address)
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
		private void Unregister(Versiport port)
		{
			if (port == null || !port.Registered)
				return;

			port.UnRegister();
		}

		/// <summary>
		/// Registers the port and then re-registers the parent.
		/// </summary>
		/// <param name="port"></param>
		private void Register(Versiport port)
		{
			if (port == null || port.Registered)
				return;

			eDeviceRegistrationUnRegistrationResponse result = port.Register();
			if (result != eDeviceRegistrationUnRegistrationResponse.Success)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to register {0} - {1}", port.GetType().Name, result);
				return;
			}

			GenericDevice parent = port.Parent as GenericDevice;
			if (parent == null)
				return;

			eDeviceRegistrationUnRegistrationResponse parentResult = parent.ReRegister();
			if (parentResult != eDeviceRegistrationUnRegistrationResponse.Success)
				Logger.AddEntry(eSeverity.Error, "Unable to register parent {0} - {1}", parent.GetType().Name, parentResult);
		}
#endif

		/// <summary>
		/// Sets the configuration mode.
		/// </summary>
		public override void SetConfiguration(eIoPortConfiguration configuration)
		{
			if (configuration == eIoPortConfiguration.None)
				throw new InvalidOperationException(string.Format("Unable to set configuration to {0}", configuration));

#if SIMPLSHARP
            try
		    {
		        eVersiportConfiguration config = s_ConfigMap[configuration];
		        m_Port.SetVersiportConfiguration(config);
				Register(m_Port);
		    }
		    catch (InvalidOperationException ex)
		    {
		        Logger.AddEntry(eSeverity.Error, "Failed to establish configuration {0} - {1}", configuration, ex.Message);
		    }

			Configuration = s_ConfigMap.GetKey(m_Port.VersiportConfiguration);
#else
            throw new NotImplementedException();
#endif
		}

		/// <summary>
		/// Sets the digital output state.
		/// </summary>
		/// <param name="digitalOut"></param>
		public override void SetDigitalOut(bool digitalOut)
		{
			if (m_Port.VersiportConfiguration != eVersiportConfiguration.DigitalOutput)
			{
				Logger.AddEntry(eSeverity.Error, "{0} failed to set digital out - not configured as a digital output", this);
				return;
			}

#if SIMPLSHARP
            try
			{
				m_Port.DigitalOut = digitalOut;
			}
			catch (InvalidOperationException e)
			{
				Logger.AddEntry(eSeverity.Error, "{0} failed to set digital out - {1}", this, e.Message);
			}
#else
            throw new NotImplementedException();
#endif
        }

        #endregion

#region Port Callbacks

#if SIMPLSHARP
        /// <summary>
        /// Subscribe to the port events.
        /// </summary>
        /// <param name="port"></param>
        private void Subscribe(Versiport port)
		{
			if (port == null)
				return;

			port.VersiportChange += PortOnVersiportChange;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(Versiport port)
		{
			if (port == null)
				return;

			port.VersiportChange += PortOnVersiportChange;
		}

		/// <summary>
		/// Called when the port raises an event.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="args"></param>
		private void PortOnVersiportChange(Versiport port, VersiportEventArgs args)
		{
			switch (args.Event)
			{
				case eVersiportEvent.DigitalInChange:
					DigitalIn = m_Port.DigitalIn;
					break;

				case eVersiportEvent.DigitalOutChange:
					DigitalOut = m_Port.DigitalOut;
					break;

				case eVersiportEvent.AnalogInChange:
					AnalogIn = m_Port.AnalogIn;
					break;

				case eVersiportEvent.VersiportConfigurationChange:
					Configuration = s_ConfigMap.GetKey(m_Port.VersiportConfiguration);
					break;
			}
		}
#endif

#endregion

#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(IoPortAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Device = m_Device;
			settings.Address = m_Address;
			settings.Configuration = Configuration;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

#if SIMPLSHARP
            SetIoPort(null, 0);
#endif
			Configuration = eIoPortConfiguration.None;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(IoPortAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

#if SIMPLSHARP
            m_Device = settings.Device;

			Versiport port = null;
			IPortParent provider = null;

			// ReSharper disable SuspiciousTypeConversion.Global
			if (m_Device != null)
				provider = factory.GetDeviceById((int)m_Device) as IPortParent;
			// ReSharper restore SuspiciousTypeConversion.Global

			if (provider == null)
				Logger.AddEntry(eSeverity.Error, "{0} is not a port provider", m_Device);
			else
			{
				try
				{
					port = provider.GetIoPort(settings.Address);
				}
				catch (Exception e)
				{
					Logger.AddEntry(eSeverity.Error, e, "Unable to get IOPort from device {0} at address {1}", m_Device,
					                settings.Address);
				}
			}

			if (provider != null && port == null)
				Logger.AddEntry(eSeverity.Error, "No IO Port at device {0} address {1}", m_Device, settings.Address);

			SetIoPort(port, settings.Address);

			if (settings.Configuration != eIoPortConfiguration.None)
				SetConfiguration(settings.Configuration);
#else
            throw new NotImplementedException();
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

#endregion
	}
}
