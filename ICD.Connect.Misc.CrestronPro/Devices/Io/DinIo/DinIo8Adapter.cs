using System;
using ICD.Common.Logging.LoggingContexts;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.CrestronPro.Cresnet;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.GeneralIO;
#endif
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Settings;
#if SIMPLSHARP

#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.DinIo
{
#if SIMPLSHARP
	public sealed class DinIo8Adapter : AbstractDevice<DinIo8AdapterSettings>, IPortParent, ICresnetDevice
#else
    public sealed class DinIo8Adapter : AbstractDevice<DinIo8AdapterSettings>
#endif
	{
		private readonly CresnetInfo m_CresnetInfo;

		public CresnetInfo CresnetInfo { get { return m_CresnetInfo; } }

#if SIMPLSHARP
		private DinIo8 m_PortsDevice;
#endif

		public DinIo8Adapter()
		{
			m_CresnetInfo = new CresnetInfo();
		}

#region Methods

#if SIMPLSHARP
		/// <summary>
		/// Sets the wrapped device.
		/// </summary>
		/// <param name="device"></param>
		public void SetDevice(DinIo8 device)
		{
			if (device == m_PortsDevice)
				return;

			Unsubscribe(m_PortsDevice);

			if (m_PortsDevice != null)
				GenericBaseUtils.TearDown(m_PortsDevice);

			m_PortsDevice = device;

			eDeviceRegistrationUnRegistrationResponse result;
			if (m_PortsDevice != null && !GenericBaseUtils.SetUp(m_PortsDevice, this, out result))
				Logger.Log(eSeverity.Error, "Unable to register {0} - {1}", m_PortsDevice.GetType().Name, result);

			Subscribe(m_PortsDevice);
			UpdateCachedOnlineStatus();
		}
#endif

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if SIMPLSHARP
			return m_PortsDevice != null && m_PortsDevice.IsOnline;
#else
            return false;
#endif
		}

#if SIMPLSHARP
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public ComPort GetComPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(ComPort).Name);
			throw new NotSupportedException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public IROutputPort GetIrOutputPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(IROutputPort).Name);
			throw new NotSupportedException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public Relay GetRelayPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Relay).Name);
			throw new NotSupportedException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public Versiport GetIoPort(int address)
		{
			if (m_PortsDevice == null)
				throw new InvalidOperationException(string.Format("{0} has no internal device", this));

			if (m_PortsDevice.VersiPorts.Contains((uint)address))
				return m_PortsDevice.VersiPorts[(uint)address];

			string message = string.Format("{0} has no {1} with address {2}", this, typeof(Versiport).Name, address);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public DigitalInput GetDigitalInputPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(DigitalInput).Name);
			throw new NotSupportedException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="io"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public Cec GetCecPort(eInputOuptut io, int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Cec).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}
#endif

#endregion

#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(DinIo8AdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			CresnetInfo.CopySettings(settings);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			CresnetInfo.ClearSettings();

#if SIMPLSHARP
			SetDevice(null);
#endif
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(DinIo8AdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			CresnetInfo.ApplySettings(settings);

#if SIMPLSHARP
			DinIo8 device = null;

			if (m_CresnetInfo.CresnetId == null || !CresnetUtils.IsValidId(m_CresnetInfo.CresnetId.Value))
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} - CresnetId {1} is out of range",
						   typeof(DinIo8).Name, m_CresnetInfo.CresnetId);
			}
			else
			{
				try
				{
					device = CresnetUtils.InstantiateCresnetDevice(m_CresnetInfo.CresnetId.Value,
																   m_CresnetInfo.BranchId,
																   m_CresnetInfo.ParentId,
					                                               factory,
					                                               cresnetId => new DinIo8(cresnetId, ProgramInfo.ControlSystem),
					                                               (cresnetId, branch) => new DinIo8(cresnetId, branch));
				}
				catch (ArgumentException e)
				{
					Logger.Log(eSeverity.Error, e, "Failed to instantiate {0} with Cresnet ID {1} - {2}",
							   typeof(DinIo8).Name, m_CresnetInfo.CresnetId, e.Message);
				}
			}

			SetDevice(device);
#endif
		}

#endregion

#region Device Callbacks

#if SIMPLSHARP
		/// <summary>
		/// Subscribe to the device events.
		/// </summary>
		/// <param name="portsDevice"></param>
		private void Subscribe(DinIo8 portsDevice)
		{
			if (portsDevice == null)
				return;

			portsDevice.OnlineStatusChange += PortsDeviceOnLineStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="portsDevice"></param>
		private void Unsubscribe(DinIo8 portsDevice)
		{
			if (portsDevice == null)
				return;

			portsDevice.OnlineStatusChange -= PortsDeviceOnLineStatusChange;
		}

		/// <summary>
		/// Called when the device online status changes.
		/// </summary>
		/// <param name="currentDevice"></param>
		/// <param name="args"></param>
		private void PortsDeviceOnLineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}
#endif

#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);
#if SIMPLSHARP
			CresnetDeviceConsole.BuildConsoleStatus(this, addRow);
#endif
		}

		#endregion
	}
}
