using System;
using System.Collections.Generic;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharpPro;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Ports.IrPort
{
	/// <summary>
	/// Implements a crestron IR Port as an IIrPort
	/// </summary>
	public sealed class IrPortAdapter : AbstractIrPort<IrPortAdapterSettings>
	{
		private IROutputPort m_Port;
		private readonly Queue<IrPulse> m_Queue;
		private readonly SafeTimer m_PulseTimer;

		// Used with settings
		private int? m_Device;
		private int m_Address;
		private string m_Driver;

		#region Properties

		/// <summary>
		/// Gets/sets the default pulse time in milliseconds for a PressAndRelease.
		/// </summary>
		public override ushort PulseTime { get; set; }

		/// <summary>
		/// Gets/sets the default time in milliseconds between PressAndRelease commands.
		/// </summary>
		public override ushort BetweenTime { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public IrPortAdapter()
		{
			m_Queue = new Queue<IrPulse>();

			m_PulseTimer = SafeTimer.Stopped(TimerCallbackMethod);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			m_PulseTimer.Dispose();

			// Unregister.
			SetIrPort(null, 0);

			base.DisposeFinal(disposing);
		}

		/// <summary>
		/// Sets the wrapped port instance.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="address"></param>
		[PublicAPI]
		public void SetIrPort(IROutputPort port, int address)
		{
			m_Address = address;

			if (m_Port != null && m_Port.Registered)
				m_Port.UnRegister();

			Clear();

			m_Port = port;

			// Obsolete?
			//if (m_Port != null)
			//	m_Port.Register();

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Loads the driver from the given path.
		/// </summary>
		/// <param name="path"></param>
		public override void LoadDriver(string path)
		{
			m_Driver = path;

			if (m_Port == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to load driver for null IR Port");
				return;
			}

			string fullPath = PathUtils.GetIrDriversPath(path);

			try
			{
				m_Port.LoadIRDriver(fullPath);
			}
			catch (FileNotFoundException)
			{
				Logger.AddEntry(eSeverity.Error, "IR Driver does not exist: {0}", fullPath);
			}
		}

		/// <summary>
		/// Begin sending the command.
		/// </summary>
		/// <param name="command"></param>
		public override void Press(string command)
		{
			Clear();

			PrintTx(command);
			m_Port.Press(command);
		}

		/// <summary>
		/// Stop sending the current command.
		/// </summary>
		public override void Release()
		{
			Clear();
		}

		/// <summary>
		/// Sends the command for the default pulse time.
		/// </summary>
		/// <param name="command"></param>
		public override void PressAndRelease(string command)
		{
			PressAndRelease(command, PulseTime);
		}

		/// <summary>
		/// Send the command for the given number of milliseconds.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="pulseTime"></param>
		public override void PressAndRelease(string command, ushort pulseTime)
		{
			PressAndRelease(command, pulseTime, BetweenTime);
		}

		/// <summary>
		/// Sends the command for the given number of milliseconds.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="pulseTime"></param>
		/// <param name="betweenTime"></param>
		public override void PressAndRelease(string command, ushort pulseTime, ushort betweenTime)
		{
			IrPulse pulse = new IrPulse(command, pulseTime, betweenTime);
			m_Queue.Enqueue(pulse);

			if (m_Queue.Count == 1)
				SendNext();
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(IrPortAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Device = m_Device;
			settings.Address = m_Address;
			settings.Driver = m_Driver;
			settings.PulseTime = PulseTime;
			settings.BetweenTime = BetweenTime;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_Device = 0;
			m_Driver = null;
			PulseTime = 0;
			BetweenTime = 0;
			SetIrPort(null, 0);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(IrPortAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_Device = settings.Device;

			PulseTime = settings.PulseTime;
			BetweenTime = settings.BetweenTime;

			IROutputPort port = null;
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
					port = provider.GetIrOutputPort(settings.Address);
				}
				catch (Exception e)
				{
					Logger.AddEntry(eSeverity.Error, e, "Unable to get IrPort from device {0} at address {1}", m_Device,
					                settings.Address);
				}
			}

			if (provider != null && port == null)
				Logger.AddEntry(eSeverity.Error, "No IR Port at {0} address {1}", m_Device, settings.Address);

			SetIrPort(port, settings.Address);
			LoadDriver(settings.Driver);
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
		/// Releases the current command and clears the queued commands.
		/// </summary>
		private void Clear()
		{
			if (m_Port != null)
				m_Port.Release();

			m_PulseTimer.Stop();
			m_Queue.Clear();
		}

		/// <summary>
		/// Sends the next pulse in the queue.
		/// </summary>
		private void SendNext()
		{
			IrPulse pulse = m_Queue.Peek();

			PrintTx(pulse.Command);
			m_Port.PressAndRelease(pulse.Command, pulse.PulseTime);
			m_PulseTimer.Reset(pulse.Duration);
		}

		/// <summary>
		/// Called when the pulse timer elapses.
		/// </summary>
		private void TimerCallbackMethod()
		{
			m_Port.Release();
			m_Queue.Dequeue();

			if (m_Queue.Count > 0)
				SendNext();
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

			addRow("Driver", m_Driver);
		}

		#endregion
	}
}
