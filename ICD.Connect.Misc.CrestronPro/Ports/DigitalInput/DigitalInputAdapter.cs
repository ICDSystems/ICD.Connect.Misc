using System;
using ICD.Connect.Protocol.Ports.DigitalInput;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using ICD.Connect.Misc.CrestronPro.Utils.Extensions;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.CrestronPro.Devices;
#endif
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Ports.DigitalInput
{
	public sealed class DigitalInputAdapter : AbstractDigitalInputPort<DigitalInputAdapterSettings>
	{
#if SIMPLSHARP

		private Crestron.SimplSharpPro.DigitalInput m_Port;
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
			base.DisposeFinal(disposing);

#if SIMPLSHARP
            // Unregister.
            SetDigitalInputPort(null, 0);
#endif
		}

#if SIMPLSHARP
        /// <summary>
        /// Sets the wrapped port instance.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="address"></param>
        [PublicAPI]
        public void SetDigitalInputPort(Crestron.SimplSharpPro.DigitalInput port, int address)
		{
			m_Address = address;

			Unsubscribe(m_Port);
			Unregister(m_Port);

			m_Port = port;

			Register(m_Port);
			Subscribe(m_Port);

			UpdateCachedOnlineStatus();

			State = GetState(m_Port);
		}

		/// <summary>
		/// Unregisters the given port.
		/// </summary>
		/// <param name="port"></param>
        private static void Unregister(Crestron.SimplSharpPro.DigitalInput port)
		{
			if (port == null || !port.Registered)
				return;

			port.UnRegister();
		}

		/// <summary>
		/// Registers the port and then re-registers the parent.
		/// </summary>
		/// <param name="port"></param>
        private void Register(Crestron.SimplSharpPro.DigitalInput port)
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

		#endregion

		#region Private Methods

#if SIMPLSHARP
		/// <summary>
		/// Gets digital in state for the given port.
		/// </summary>
		/// <param name="port"></param>
		/// <returns>False if the port is null, or the port is not configured for digital in.</returns>
        private static bool GetState(Crestron.SimplSharpPro.DigitalInput port)
		{
			try
			{
				return port != null && port.State;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
		}
#endif

		#endregion

		#region Port Callbacks

#if SIMPLSHARP
        /// <summary>
        /// Subscribe to the port events.
        /// </summary>
        /// <param name="port"></param>
        private void Subscribe(Crestron.SimplSharpPro.DigitalInput port)
		{
			if (port == null)
				return;

			port.StateChange += PortOnStateChange;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
        private void Unsubscribe(Crestron.SimplSharpPro.DigitalInput port)
		{
			if (port == null)
				return;

			port.StateChange -= PortOnStateChange;
		}

		/// <summary>
		/// Called when the port raises an event.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="args"></param>
        private void PortOnStateChange(Crestron.SimplSharpPro.DigitalInput port, DigitalInputEventArgs args)
		{
		    State = args.State;
		}
#endif

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(DigitalInputAdapterSettings settings)
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
            SetDigitalInputPort(null, 0);
#endif
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(DigitalInputAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

#if SIMPLSHARP
            m_Device = settings.Device;

			Crestron.SimplSharpPro.DigitalInput port = null;
			IPortParent provider = null;
			
			if (m_Device != null)
				provider = factory.GetDeviceById((int)m_Device) as IPortParent;

			if (provider == null)
				Logger.AddEntry(eSeverity.Error, "{0} is not a port provider", m_Device);
			else
			{
				try
				{
					port = provider.GetDigitalInputPort(settings.Address);
				}
				catch (Exception e)
				{
					Logger.AddEntry(eSeverity.Error, e, "Unable to get Digital Input Port from device {0} at address {1}", m_Device,
					                settings.Address);
				}
			}

			if (provider != null && port == null)
				Logger.AddEntry(eSeverity.Error, "No Digital Input Port at device {0} address {1}", m_Device, settings.Address);

			SetDigitalInputPort(port, settings.Address);
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
