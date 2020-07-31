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
		private CresnetDeviceInfo m_CresnetDeviceInfo;

		public CresnetDeviceInfo CresnetDeviceInfo { get { return m_CresnetDeviceInfo; } }
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

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_CresnetDeviceInfo.ClearSettings();

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

			m_CresnetDeviceInfo.CopySettings(settings);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(CsaPws10sHubEnetSlaveAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_CresnetDeviceInfo = new CresnetDeviceInfo(settings);
#if SIMPLSHARP
			CsaPws10sHubEnetSlave device = null;
			try
			{
				if (m_CresnetDeviceInfo.CresnetId.HasValue)
				{
					device = CresnetUtils.InstantiateCresnetDevice(m_CresnetDeviceInfo.CresnetId.Value,
																   m_CresnetDeviceInfo.BranchId,
																   m_CresnetDeviceInfo.ParentId,
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
						   typeof(CsaPws10sHubEnetSlave).Name, m_CresnetDeviceInfo.CresnetId, e.Message);
			}
			finally
			{
				SetDevice(device, m_CresnetDeviceInfo.ParentId, m_CresnetDeviceInfo.BranchId);
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