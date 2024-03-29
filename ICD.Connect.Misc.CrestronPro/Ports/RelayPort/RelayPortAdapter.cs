﻿using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Misc.CrestronPro.Extensions;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Protocol.Ports.RelayPort;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
#endif

namespace ICD.Connect.Misc.CrestronPro.Ports.RelayPort
{
	public sealed class RelayPortAdapter : AbstractRelayPort<RelayPortAdapterSettings>
	{
#if !NETSTANDARD
		private Relay m_Port;
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
#if !NETSTANDARD
			// Unregister.
			SetRelayPort(null, 0);
#endif

			base.DisposeFinal(disposing);
		}

#if !NETSTANDARD
		/// <summary>
		/// Sets the wrapped port instance.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="address"></param>
		[PublicAPI]
		public void SetRelayPort(Relay port, int address)
		{
			m_Address = address;

			Unsubscribe(m_Port);
			Unregister(m_Port);

			m_Port = port;

			Subscribe(m_Port);
			Register(m_Port);

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Unregisters the given port.
		/// </summary>
		/// <param name="port"></param>
		private void Unregister(Relay port)
		{
			if (port != null)
				PortDeviceUtils.Unregister(port);
		}

		/// <summary>
		/// Registers the port and then re-registers the parent.
		/// </summary>
		/// <param name="port"></param>
		private void Register(Relay port)
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
		/// Open the relay
		/// </summary>
		public override void Open()
		{
#if !NETSTANDARD
			if (m_Port == null)
			{
				Logger.Log(eSeverity.Error, "Unable to open relay - internal port is null");
				return;
			}

			PrintTx(() => "Opening Relay");

			m_Port.Open();
#else
			throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Close the relay
		/// </summary>
		public override void Close()
		{
#if !NETSTANDARD
			if (m_Port == null)
			{
				Logger.Log(eSeverity.Error, "Unable to close relay - internal port is null");
				return;
			}

			PrintTx(() => "Closing Relay");

			m_Port.Close();
#else
			throw new NotSupportedException();
#endif
		}

		#endregion

		#region Port Callbacks

#if !NETSTANDARD
		/// <summary>
		/// Subscribe to the relay events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(Relay port)
		{
			if (port == null)
				return;

			port.StateChange += PortOnStateChange;

			GenericBase parent = port.Parent as GenericBase;
			if (parent != null)
				parent.OnlineStatusChange += ParentOnOnlineStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the relay events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(Relay port)
		{
			if (port == null)
				return;

			port.StateChange -= PortOnStateChange;

			GenericBase parent = port.Parent as GenericBase;
			if (parent != null)
				parent.OnlineStatusChange -= ParentOnOnlineStatusChange;
		}

		/// <summary>
		/// Called when the relay opens/closes.
		/// </summary>
		/// <param name="relay"></param>
		/// <param name="args"></param>
		private void PortOnStateChange(Relay relay, RelayEventArgs args)
		{
			PrintRx(() => "Relay state changed to " + args.State);
			Closed = args.State == Relay.Relay_State.Closed;
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
		protected override void CopySettingsFinal(RelayPortAdapterSettings settings)
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
			SetRelayPort(null, 0);
#endif
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(RelayPortAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

#if !NETSTANDARD
			m_Device = settings.Device;

			Relay port = null;
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
					port = provider.GetRelayPort(settings.Address);
				}
				catch (Exception e)
				{
					Logger.Log(eSeverity.Error, "Unable to get Relay Port from device {0} at address {1} - {2}", m_Device,
					                settings.Address, e.Message);
				}
			}

			if (provider != null && port == null)
				Logger.Log(eSeverity.Error, "No Relay Port at device {0} address {1}", m_Device, settings.Address);

			SetRelayPort(port, settings.Address);
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
