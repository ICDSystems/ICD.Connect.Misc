using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharp.CrestronIO;
using ICD.Common.Utils.Services.Logging;
using Crestron.SimplSharpPro;
using ICD.Connect.Misc.CrestronPro.Extensions;
#endif

namespace ICD.Connect.Misc.CrestronPro.Ports.IrPort
{
	/// <summary>
	/// Implements a crestron IR Port as an IIrPort
	/// </summary>
	public sealed class IrPortAdapter : AbstractIrPort<IrPortAdapterSettings>
	{
		private const ushort DEFAULT_PULSE_TIME = 100;
		private const ushort DEFAULT_BETWEEN_TIME = 750;

#if !NETSTANDARD
		private IROutputPort m_Port;
#endif
		private readonly IrDriverProperties m_IrDriverProperties;

		private readonly SafeCriticalSection m_PressSection;

		private string m_LoadedDriverPath;

		// Used with settings
		private int? m_Device;
		private int m_Address;

		#region Properties

		/// <summary>
		/// Gets the IR Driver configuration properties.
		/// </summary>
		public override IIrDriverProperties IrDriverProperties { get { return m_IrDriverProperties; } }

		/// <summary>
		/// Gets the path to the loaded IR driver.
		/// </summary>
		public override string DriverPath { get { return m_LoadedDriverPath; } }

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
			m_IrDriverProperties = new IrDriverProperties();

			m_PressSection = new SafeCriticalSection();

			PulseTime = DEFAULT_PULSE_TIME;
			BetweenTime = DEFAULT_BETWEEN_TIME;
		}

		#endregion

		#region Methods

#if !NETSTANDARD
		/// <summary>
		/// Sets the wrapped port instance.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="address"></param>
		[PublicAPI]
		public void SetIrPort(IROutputPort port, int address)
		{
			Release();

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
		private void Unregister(IROutputPort port)
		{
			if (port != null)
				PortDeviceUtils.Unregister(port);
		}

		/// <summary>
		/// Re-registers the parent.
		/// </summary>
		/// <param name="port"></param>
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
#endif

		/// <summary>
		/// Loads the driver from the given path.
		/// </summary>
		/// <param name="path"></param>
		public override void LoadDriver(string path)
		{
#if !NETSTANDARD
			if (m_Port == null)
			{
				Logger.Log(eSeverity.Error, "Unable to load driver - internal port is null");
				return;
			}

			string fullPath = GetIrDriversPath(path);

			try
			{
				m_Port.UnloadAllIRDrivers();
				m_Port.LoadIRDriver(fullPath);

				m_LoadedDriverPath = fullPath;
			}
			catch (FileNotFoundException)
			{
				m_LoadedDriverPath = null;

				Logger.Log(eSeverity.Error, "Unable to load driver - file does not exist: {0}", fullPath);
			}
#else
			throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Gets the loaded IR commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<string> GetCommands()
		{
#if !NETSTANDARD
			return m_Port == null ? Enumerable.Empty<string>() : m_Port.AvailableIRCmds();
#else
			throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Override to implement the press logic.
		/// </summary>
		/// <param name="command"></param>
		protected override void PressFinal(string command)
		{
#if !NETSTANDARD
			if (m_Port == null)
			{
				Logger.Log(eSeverity.Error, "Unable to send command - internal port is null");
				return;
			}

			m_PressSection.Enter();

			try
			{
				if (!m_Port.IsIRCommandAvailable(command))
				{
					Logger.Log(eSeverity.Error, "Unable to send command - No command {0}", StringUtils.ToRepresentation(command));
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
		/// Override to implement the release logic.
		/// </summary>
		protected override void ReleaseFinal()
		{
#if !NETSTANDARD
			if (m_Port != null)
				m_Port.Release();
#endif
		}

		#endregion

		#region Port Callbacks

#if !NETSTANDARD
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
		}
#endif

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

		/// <summary>
		/// Searches the application path, program config path and common config path to
		/// find the first IR driver that exists with the given local path.
		/// </summary>
		/// <param name="localPath"></param>
		/// <returns></returns>
		private static string GetIrDriversPath(string localPath)
		{
			return PathUtils.GetDefaultConfigPath(new[] {"IRDrivers", localPath});
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
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_Device = 0;

#if !NETSTANDARD
			SetIrPort(null, 0);
#endif
			PulseTime = DEFAULT_PULSE_TIME;
			BetweenTime = DEFAULT_BETWEEN_TIME;
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

			SetIrPort(port, settings.Address);

			ApplyConfiguration();
#endif
		}

		#endregion
	}
}
