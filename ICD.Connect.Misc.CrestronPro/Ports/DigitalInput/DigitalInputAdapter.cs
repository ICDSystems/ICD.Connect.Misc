using System;
using System.Collections.Generic;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Misc.CrestronPro.Extensions;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Protocol.Ports.DigitalInput;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using ICD.Common.Properties;
using ICD.Connect.Misc.CrestronPro.Devices;
#endif

namespace ICD.Connect.Misc.CrestronPro.Ports.DigitalInput
{
	public sealed class DigitalInputAdapter : AbstractDigitalInputPort<DigitalInputAdapterSettings>
	{
#if !NETSTANDARD
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

#if !NETSTANDARD
			// Unregister.
			SetDigitalInputPort(null, 0);
#endif
		}

#if !NETSTANDARD
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
			if (port != null)
				PortDeviceUtils.Unregister(port);
		}

		/// <summary>
		/// Registers the port and then re-registers the parent.
		/// </summary>
		/// <param name="port"></param>
		private void Register(Crestron.SimplSharpPro.DigitalInput port)
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

		#endregion

		#region Private Methods

#if !NETSTANDARD
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

#if !NETSTANDARD
		/// <summary>
		/// Subscribe to the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(Crestron.SimplSharpPro.DigitalInput port)
		{
			if (port == null)
				return;

			port.StateChange += PortOnStateChange;

			GenericBase parent = port.Parent as GenericBase;
			if (parent != null)
				parent.OnlineStatusChange += ParentOnOnlineStatusChange;
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

			GenericBase parent = port.Parent as GenericBase;
			if (parent != null)
				parent.OnlineStatusChange -= ParentOnOnlineStatusChange;
		}

		/// <summary>
		/// Called when the port raises an event.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="args"></param>
		private void PortOnStateChange(Crestron.SimplSharpPro.DigitalInput port, DigitalInputEventArgs args)
		{
			PrintRx(() => "Digital Input state changed to " + args.State);
			State = args.State;
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

#if !NETSTANDARD
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

#if !NETSTANDARD
			m_Device = settings.Device;

			Crestron.SimplSharpPro.DigitalInput port = null;
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
					port = provider.GetDigitalInputPort(settings.Address);
				}
				catch (Exception e)
				{
					Logger.Log(eSeverity.Error, "Unable to get Digital Input Port from device {0} at address {1}:{2}", m_Device,
					                settings.Address, e);
				}
			}

			if (provider != null && port == null)
				Logger.Log(eSeverity.Error, "No Digital Input Port at device {0} address {1}", m_Device, settings.Address);

			SetDigitalInputPort(port, settings.Address);
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
			return m_Port != null && m_Port.GetParentOnline();
#else
			return false;
#endif
		}

		#endregion
	}
}
