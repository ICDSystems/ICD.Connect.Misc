using System;
using System.Collections.Generic;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro.DM;
#endif

namespace ICD.Connect.Misc.CrestronPro.Ports.CecPort
{
	/// <summary>
	/// This provides the krang adapter for a CecPort
	/// </summary>
	public sealed class CecPortAdapter : AbstractSerialPort<CecPortAdapterSettings>
	{

#if SIMPLSHARP
		private Cec m_Port;
#endif
		//Used with settings
		private int? m_Device;
		private int m_Address;
		private eInputOuptut m_Io;




		#region Methods
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
		/// Returns the connection state of the port
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsConnectedState()
		{
			return m_Port != null;
		}

		/// <summary>
		/// Sends the data to the remote endpoint.
		/// </summary>
		protected override bool SendFinal(string data)
		{
#if SIMPLSHARP
			if (m_Port == null)
			{
				Log(eSeverity.Error, "Unable to send - internal port is null");
				return false;
			}

			PrintTx(data);
			m_Port.Send.StringValue = data;

			return true;
#else
            throw new NotSupportedException();
#endif
		}

		private void ReceiveMessage()
		{
			string data;

			data = m_Port.Received.StringValue;

			if (!String.IsNullOrEmpty(data))
			{
				PrintRx(data);      
				Receive(data);
			}
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);
#if SIMPLSHARP
			SetPort(null);
#endif
		}

#if SIMPLSHARP

		private void SetPort(Cec port)
		{
			if (port == m_Port)
				return;

			Unsubscribe(m_Port);

			m_Port = port;

			Subscribe(m_Port);
		}

		private void Subscribe(Cec port)
		{
			if (port == null)
				return;

			port.CecChange += PortOnCecChange;
		}

		private void Unsubscribe(Cec port)
		{
			if (port == null)
				return;

			port.CecChange -= PortOnCecChange;
		}

		private void PortOnCecChange(Cec cecDevice, CecEventArgs args)
		{
			if (args.EventId == CecEventIds.CecMessageReceivedEventId)
			{
				ReceiveMessage();
			}
		}

#endif

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(CecPortAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Device = m_Device;
			settings.Io = m_Io;
			settings.Address = m_Address;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_Device = 0;
			m_Io = eInputOuptut.Output;
			m_Address = 1;

#if SIMPLSHARP
			SetPort(null);
#endif
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(CecPortAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

#if SIMPLSHARP
			m_Device = settings.Device;

			Cec port = null;
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
				Log(eSeverity.Error, "{0} is not a {1}", m_Device, typeof(IPortParent).Name);
			else
			{
				try
				{
					port = provider.GetCecPort(settings.Io, settings.Address);
					if (port == null)
						Log(eSeverity.Error, "No Cec Port at {0} address {1}:{2}", m_Device, settings.Io, settings.Address);
				}
				catch (Exception e)
				{
					Log(eSeverity.Error, "Unable to get CecPort from device {0} at address {1}:{2} - {3}", m_Device, settings.Io,
						settings.Address, e.Message);
				}
			}

			if (provider != null && port == null)
				Log(eSeverity.Error, "No Cec Port at {0} address {1}:{2}", m_Device, settings.Io, settings.Address);

			m_Io = settings.Io;
			m_Address = settings.Address;

			SetPort(port);
#endif
		}

		#endregion
	}
}