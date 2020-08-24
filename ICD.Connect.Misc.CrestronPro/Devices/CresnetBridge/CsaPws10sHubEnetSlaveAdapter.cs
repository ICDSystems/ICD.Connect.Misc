using System;
using ICD.Common.Logging.LoggingContexts;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.CrestronPro.Cresnet;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro.GeneralIO;
#endif
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Misc.CrestronPro.Utils;

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
	public sealed class CsaPws10sHubEnetSlaveAdapter : AbstractDevice<CsaPws10sHubEnetSlaveAdapterSettings>, ICsaPws10sHubEnetAdapter, ICresnetDevice
	{
#if SIMPLSHARP
		private CsaPws10sHubEnetSlave m_Device;
#endif
		private readonly CresnetInfo m_CresnetInfo;

		public CresnetInfo CresnetInfo { get { return m_CresnetInfo; } }
#if SIMPLSHARP

		private void SetDevice(CsaPws10sHubEnetSlave device, int? parentId, int? branchId)
		{
			if (device == m_Device)
				return;

			m_Device.UnRegister();
			m_Device = device;
			m_Device.Register();
		}
#endif

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if SIMPLSHARP
			return m_Device.IsOnline;
#else
			return false;
#endif
		}

		public CsaPws10sHubEnetSlaveAdapter()
		{
			m_CresnetInfo = new CresnetInfo();
		}

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			CresnetInfo.ClearSettings();

#if SIMPLSHARP
			m_Device = null;
#endif
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(CsaPws10sHubEnetSlaveAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			CresnetInfo.CopySettings(settings);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(CsaPws10sHubEnetSlaveAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			CresnetInfo.ApplySettings(settings);

#if SIMPLSHARP
			CsaPws10sHubEnetSlave device = null;
			try
			{
				if (m_CresnetInfo.CresnetId.HasValue)
				{
					device = CresnetUtils.InstantiateCresnetDevice(m_CresnetInfo.CresnetId.Value,
																   m_CresnetInfo.BranchId,
																   m_CresnetInfo.ParentId,
					                                               factory,
					                                               cresnetId =>
					                                               new CsaPws10sHubEnetSlave(cresnetId, ProgramInfo.ControlSystem),
					                                               (cresnetId, branchId) =>
					                                               new CsaPws10sHubEnetSlave(cresnetId, branchId));
				}
				else
				{
					Logger.Log(eSeverity.Error, "Failed to instantiate {0} - Settings requires a CresnetID", typeof(CsaPws10sHubEnetSlave).Name);
				}
			}
			catch (ArgumentException e)
			{
				Logger.Log(eSeverity.Error, e, "Failed to instantiate {0} with Cresnet ID {1} - {2}",
						   typeof(CsaPws10sHubEnetSlave).Name, m_CresnetInfo.CresnetId, e.Message);
			}
			finally
			{
				SetDevice(device, m_CresnetInfo.ParentId, m_CresnetInfo.BranchId);
			}
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

			CresnetDeviceConsole.BuildConsoleStatus(this, addRow);
		}

		#endregion
	}
}