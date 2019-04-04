using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Settings.Core;
#if SIMPLSHARP
using ICD.Common.Properties;
using Crestron.SimplSharp.CrestronIO;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using Crestron.SimplSharpPro;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Misc.CrestronPro.Extensions;
#endif

namespace ICD.Connect.Misc.CrestronPro.Ports.IrPort
{
	/// <summary>
	/// Implements a crestron IR Port as an IIrPort
	/// </summary>
	public sealed class IrPortAdapter : AbstractIrPort<IrPortAdapterSettings>
	{
#if SIMPLSHARP
		private IROutputPort m_Port;
#endif
		private readonly Queue<IrPulse> m_Queue;
		private readonly SafeTimer m_PulseTimer;

		private readonly SafeCriticalSection m_PressSection;

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
			m_PressSection = new SafeCriticalSection();

			m_PulseTimer = SafeTimer.Stopped(PulseElapseCallback);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			m_PulseTimer.Dispose();

#if SIMPLSHARP
			// Unregister.
			SetIrPort(null, 0);
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
		public void SetIrPort(IROutputPort port, int address)
		{
			m_Address = address;

			Unregister(m_Port);
			Clear();

			m_Port = port;
			Register(m_Port);

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Unregisters the given port.
		/// </summary>
		/// <param name="port"></param>
		private void Unregister(IROutputPort port)
		{
			if (port == null || !port.Registered)
				return;

			port.UnRegister();
		}

		/// <summary>
		/// Re-registers the parent.
		/// </summary>
		/// <param name="port"></param>
		private void Register(IROutputPort port)
		{
			if (port == null)
				return;

			GenericDevice parent = port.Parent as GenericDevice;
			if (parent == null)
				return;

			eDeviceRegistrationUnRegistrationResponse parentResult = parent.ReRegister();
			if (parentResult != eDeviceRegistrationUnRegistrationResponse.Success)
				Log(eSeverity.Error, "Unable to register parent {0} - {1}", parent.GetType().Name, parentResult);
		}
#endif

		/// <summary>
		/// Loads the driver from the given path.
		/// </summary>
		/// <param name="path"></param>
		public override void LoadDriver(string path)
		{
#if SIMPLSHARP
			m_Driver = path;

			if (m_Port == null)
			{
				Log(eSeverity.Error, "Unable to load driver - internal port is null");
				return;
			}

			string fullPath = GetIrDriversPath(path);

			try
			{
				m_Port.UnloadAllIRDrivers();
				m_Port.LoadIRDriver(fullPath);
			}
			catch (FileNotFoundException)
			{
				Log(eSeverity.Error, "Unable to load driver - file does not exist: {0}", fullPath);
			}
#else
            throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Begin sending the command.
		/// </summary>
		/// <param name="command"></param>
		public override void Press(string command)
		{
#if SIMPLSHARP
			if (m_Port == null)
			{
				Log(eSeverity.Error, "Unable to send command - internal port is null");
				return;
			}

			m_PressSection.Enter();

			try
			{
				Clear();

				if (!m_Port.IsIRCommandAvailable(command))
				{
					Log(eSeverity.Error, "Unable to send command - No command {0}", StringUtils.ToRepresentation(command));
					return;
				}

				PrintTx(command);
				m_Port.Press(command);
			}
			finally
			{
				m_PressSection.Leave();
			}
#else
            throw new NotSupportedException();
#endif
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

			m_PressSection.Enter();

			try
			{
				m_Queue.Enqueue(pulse);

				if (m_Queue.Count == 1)
					SendNext();
			}
			finally
			{
				m_PressSection.Leave();
			}
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

#if SIMPLSHARP
			SetIrPort(null, 0);
#endif
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

#if SIMPLSHARP
			IROutputPort port = null;
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
					port = provider.GetIrOutputPort(settings.Address);
				}
				catch (Exception e)
				{
					Log(eSeverity.Error, "Unable to get IrPort from device {0} at address {1} - {2}", m_Device,
					    settings.Address, e.Message);
				}
			}

			if (provider != null && port == null)
				Log(eSeverity.Error, "No IR Port at {0} address {1}", m_Device, settings.Address);

			SetIrPort(port, settings.Address);

			if (!string.IsNullOrEmpty(settings.Driver))
				LoadDriver(settings.Driver);
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
		/// Releases the current command and clears the queued commands.
		/// </summary>
		private void Clear()
		{
#if SIMPLSHARP
			m_PressSection.Enter();

			try
			{
				if (m_Port != null)
					m_Port.Release();

				m_PulseTimer.Stop();
				m_Queue.Clear();
			}
			finally
			{
				m_PressSection.Leave();
			}
#else
            throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Sends the next pulse in the queue.
		/// </summary>
		private void SendNext()
		{
#if SIMPLSHARP
			m_PressSection.Enter();

			try
			{
				IrPulse pulse;
				if (!m_Queue.Dequeue(out pulse))
					return;

				if (m_Port == null)
				{
					Log(eSeverity.Error, "Unable to send command - internal port is null");
					Clear();
					return;
				}

				if (!m_Port.IsIRCommandAvailable(pulse.Command))
				{
					Log(eSeverity.Error, "Unable to send command - No command {0}", StringUtils.ToRepresentation(pulse.Command));
					SendNext();
					return;
				}

				PrintTx(pulse.Command);
				m_Port.PressAndRelease(pulse.Command, pulse.PulseTime);

				m_PulseTimer.Reset(pulse.Duration);
			}
			finally
			{
				m_PressSection.Leave();
			}
#else
            throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Called when the pulse timer elapses.
		/// </summary>
		private void PulseElapseCallback()
		{
#if SIMPLSHARP
			m_PressSection.Enter();

			try
			{
				if (m_Queue.Count > 0)
					SendNext();
			}
			finally
			{
				m_PressSection.Leave();
			}
#else
            throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Searches the application path, program config path and common config path to
		/// find the first IR driver that exists with the given local path.
		/// </summary>
		/// <param name="localPath"></param>
		/// <returns></returns>
		public static string GetIrDriversPath(string localPath)
		{
			return PathUtils.GetDefaultConfigPath(new[] {"IRDrivers", localPath});
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

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand consoleCommand in GetBaseConsoleCommands())
				yield return consoleCommand;

			yield return new ConsoleCommand("PrintCommands", "Prints the available commands", () => PrintCommands());
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		private string PrintCommands()
		{
			TableBuilder builder = new TableBuilder("Command");

#if SIMPLSHARP
			if (m_Port != null)
			{
				foreach (var command in m_Port.AvailableIRCmds())
					builder.AddRow(command);
			}
#endif

			return builder.ToString();
		}

		#endregion
	}
}
