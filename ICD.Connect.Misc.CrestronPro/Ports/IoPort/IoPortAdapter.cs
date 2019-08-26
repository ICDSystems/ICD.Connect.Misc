using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.CrestronPro.Extensions;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Ports.IoPort;
using ICD.Connect.Settings.Core;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.CrestronPro.Devices;
#endif

namespace ICD.Connect.Misc.CrestronPro.Ports.IoPort
{
	public sealed class IoPortAdapter : AbstractIoPort<IoPortAdapterSettings>
	{
#if SIMPLSHARP

		private const int PORT_RECHECK_TIME = 250;

		private static readonly Dictionary<eIoPortConfiguration, eVersiportConfiguration> s_ConfigMap =
			new Dictionary<eIoPortConfiguration, eVersiportConfiguration>
			{
				{eIoPortConfiguration.None, eVersiportConfiguration.NotSet},
				{eIoPortConfiguration.AnalogIn, eVersiportConfiguration.AnalogInput},
				{eIoPortConfiguration.DigitalIn, eVersiportConfiguration.DigitalInput},
				{eIoPortConfiguration.DigitalOut, eVersiportConfiguration.DigitalOutput}
			};

		private Versiport m_Port;

		private bool m_RequestedState;

		private bool m_PortStateBusy;

		private readonly SafeTimer m_PortRecheckTimer;
#endif

		// Used with settings
		private int? m_Device;
		private int m_Address;

		private readonly SafeCriticalSection m_SetDigitalSection;

		public IoPortAdapter()
		{
			m_SetDigitalSection = new SafeCriticalSection();
#if SIMPLSHARP
			m_PortRecheckTimer = SafeTimer.Stopped(PortRecheck);
#endif
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

#if SIMPLSHARP
			// Unregister.
			SetIoPort(null, 0);
#endif
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

			DigitalIn = GetDigitalIn(m_Port);
			DigitalOut = GetDigitalOut(m_Port);
			AnalogIn = GetAnalogIn(m_Port);
			Configuration = GetConfiguration(m_Port);
		}

		/// <summary>
		/// Unregisters the given port.
		/// </summary>
		/// <param name="port"></param>
		private void Unregister(Versiport port)
		{
			if (port != null)
				PortDeviceUtils.Unregister(port);
		}

		/// <summary>
		/// Registers the port and then re-registers the parent.
		/// </summary>
		/// <param name="port"></param>
		private void Register(Versiport port)
		{
			try
			{
				if (port != null)
					PortDeviceUtils.Register(port);
			}
			catch (InvalidOperationException e)
			{
				Log(eSeverity.Error, "Error registering port - {0}", e.Message);
			}
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
			if (m_Port == null)
			{
				Log(eSeverity.Error, "Failed to set configuration - no internal port");
				return;
			}

			try
			{
				eVersiportConfiguration config = s_ConfigMap[configuration];
				m_Port.SetVersiportConfiguration(config);
				Register(m_Port);
			}
			catch (InvalidOperationException ex)
			{
				Log(eSeverity.Error, "Failed to establish configuration {0} - {1}", configuration, ex.Message);
			}

			Configuration = GetConfiguration(m_Port);


			if (DebugTx != eDebugMode.Off)
				PrintTx("Configuration - " + Configuration);
#else
			throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Sets the digital output state.
		/// </summary>
		/// <param name="digitalOut"></param>
		public override void SetDigitalOut(bool digitalOut)
		{
#if SIMPLSHARP

			m_RequestedState = digitalOut;

			m_SetDigitalSection.Enter();
			try
			{
				if (m_PortStateBusy)
				{
					m_PortRecheckTimer.Reset(PORT_RECHECK_TIME);
					return;
				}

				m_PortStateBusy = true;

				if (m_Port == null)
				{
					Log(eSeverity.Error, "Failed to set digital out - no port assigned");
					return;
				}

				if (m_Port.VersiportConfiguration != eVersiportConfiguration.DigitalOutput)
				{
					Log(eSeverity.Error, "Failed to set digital out - not configured as a digital output");
					return;
				}

				try
				{
					m_Port.DigitalOut = digitalOut;

					// Not all devices (e.g. DIN-IO8 give DigitalOut feedback, so lets cache it)
					DigitalOut = digitalOut;
				}
				catch (InvalidOperationException e)
				{
					Log(eSeverity.Error, "Failed to set digital out - {0}", e.Message);
				}

				if (DebugTx != eDebugMode.Off)
					PrintTx("Digital Out - " + DigitalOut);

				m_PortRecheckTimer.Reset(PORT_RECHECK_TIME);
			}
			finally
			{
				m_SetDigitalSection.Leave();
			}
#else
			throw new NotSupportedException();
#endif
		}

		#endregion

		#region Private Methods

#if SIMPLSHARP

		/// <summary>
		/// This abominiation is to fix versiports not updating
		/// fast enough in some situations.
		/// </summary>
		private void PortRecheck()
		{
			bool needsSet = false;

			m_SetDigitalSection.Enter();
			try
			{
				if (m_RequestedState != DigitalOut)
					needsSet = true;
				m_PortStateBusy = false;
			}
			finally
			{
				m_SetDigitalSection.Leave();
			}

			if (needsSet)
				SetDigitalOut(m_RequestedState);
		}

		/// <summary>
		/// Gets digital in state for the given port.
		/// </summary>
		/// <param name="port"></param>
		/// <returns>False if the port is null, or the port is not configured for digital in.</returns>
		private static bool GetDigitalIn(Versiport port)
		{
			try
			{
				return port != null && port.DigitalIn;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
		}

		/// <summary>
		/// Gets digital out state for the given port.
		/// </summary>
		/// <param name="port"></param>
		/// <returns>False if the port is null, or the port is not configured for digital out.</returns>
		private static bool GetDigitalOut(Versiport port)
		{
			try
			{
				return port != null && port.DigitalOut;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
		}

		/// <summary>
		/// Gets analog in state for the given port.
		/// </summary>
		/// <param name="port"></param>
		/// <returns>False if the port is null, or the port is not configured for analog in.</returns>
		private static ushort GetAnalogIn(Versiport port)
		{
			try
			{
				return port == null ? (ushort)0 : port.AnalogIn;
			}
			catch (InvalidOperationException)
			{
				return 0;
			}
		}

		/// <summary>
		/// Gets the configuration for the given port.
		/// </summary>
		/// <param name="port"></param>
		/// <returns>None if the port is null.</returns>
		private static eIoPortConfiguration GetConfiguration(Versiport port)
		{
			return port == null ? eIoPortConfiguration.None : s_ConfigMap.GetKey(port.VersiportConfiguration);
		}
#endif

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

			GenericBase parent = port.Parent as GenericBase;
			if (parent != null)
				parent.OnlineStatusChange += ParentOnOnlineStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(Versiport port)
		{
			if (port == null)
				return;

			port.VersiportChange -= PortOnVersiportChange;

			GenericBase parent = port.Parent as GenericBase;
			if (parent != null)
				parent.OnlineStatusChange -= ParentOnOnlineStatusChange;
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
					DigitalIn = GetDigitalIn(m_Port);
					if (DebugRx != eDebugMode.Off)
						PrintRx("Digital In - " + DigitalIn);
					break;

				case eVersiportEvent.DigitalOutChange:
					DigitalOut = GetDigitalOut(m_Port);
					break;

				case eVersiportEvent.AnalogInChange:
					AnalogIn = GetAnalogIn(m_Port);
					if (DebugRx != eDebugMode.Off)
						PrintRx("Analog In - " + AnalogIn);
					break;

				case eVersiportEvent.VersiportConfigurationChange:
					Configuration = GetConfiguration(m_Port);
					break;
			}
		}

		private void ParentOnOnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
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

			if (m_Device != null)
			{
				try
				{
					provider = factory.GetDeviceById((int)m_Device) as IPortParent;
				}
				catch (KeyNotFoundException)
				{
					Log(eSeverity.Error, "No device with id {0}", m_Device);
				}
			}

			if (provider == null)
				Log(eSeverity.Error, "{0} is not a port provider", m_Device);
			else
			{
				try
				{
					port = provider.GetIoPort(settings.Address);
				}
				catch (Exception e)
				{
					Log(eSeverity.Error, "Unable to get IOPort from device {0} at address {1} - {2}", m_Device,
					    settings.Address, e.Message);
				}
			}

			if (provider != null && port == null)
				Log(eSeverity.Error, "No IO Port at device {0} address {1}", m_Device, settings.Address);

			SetIoPort(port, settings.Address);
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
			return m_Port != null && m_Port.GetIsRegisteredAndParentOnline();
#else
			return false;
#endif
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);
			addRow("Address", m_Address);

#if SIMPLSHARP
			addRow("Port Registration", m_Port != null && m_Port.Registered);
#endif
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

#if SIMPLSHARP
			yield return new ConsoleCommand("Register", "Register the port", () => Register(m_Port));
			yield return new ConsoleCommand("Unregister", "Unregister the port", () => Unregister(m_Port));
#endif
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
