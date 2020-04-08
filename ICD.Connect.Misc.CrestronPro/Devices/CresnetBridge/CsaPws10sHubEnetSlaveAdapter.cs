using System;
using ICD.Common.Logging.LoggingContexts;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro.GeneralIO;
#endif
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Misc.CrestronPro.Utils;

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
	public sealed class CsaPws10sHubEnetSlaveAdapter : AbstractDevice<CsaPws10sHubEnetSlaveAdapterSettings>, ICsaPws10sHubEnetAdapter
	{
#if SIMPLSHARP
		private CsaPws10sHubEnetSlave m_Device;
#endif
		private int? m_ParentId;
		private int? m_BranchId;
#if SIMPLSHARP

		private void SetDevice(CsaPws10sHubEnetSlave device, int? parentId, int? branchId)
		{
			if (device == m_Device)
				return;

			m_Device.UnRegister();

			m_Device = device;
			m_ParentId = parentId;
			m_BranchId = branchId;

			m_Device.Register();
		}
#endif
		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();
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

#if SIMPLSHARP
			settings.CresnetId = m_Device == null ? (byte?)null : (byte)m_Device.ID;
			settings.BranchId = m_BranchId;
			settings.ParentId = m_ParentId;
#endif
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(CsaPws10sHubEnetSlaveAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);
#if SIMPLSHARP
			CsaPws10sHubEnetSlave device = null;
			try
			{
				if (settings.CresnetId.HasValue)
				{
					device = CresnetUtils.InstantiateCresnetDevice(settings.CresnetId.Value,
					                                               settings.BranchId,
					                                               settings.ParentId,
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
				           typeof(CsaPws10sHubEnetSlave).Name, settings.CresnetId, e.Message);
			}
			finally
			{
				SetDevice(device, settings.ParentId, settings.BranchId);
			}
#endif
		}

		#endregion

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
	}
}