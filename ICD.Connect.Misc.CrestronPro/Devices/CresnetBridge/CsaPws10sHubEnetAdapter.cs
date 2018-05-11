using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.GeneralIO;
#endif
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Misc.CrestronPro.Utils;

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
	public sealed class CsaPws10sHubEnetAdapter : AbstractDevice<CsaPws10sHubEnetAdapterSettings>, ICsaPws10sHubEnetAdapter
	{
#if SIMPLSHARP
		private CsaPws10sHubEnetBase m_Device;
#endif
		private int? m_ParentId;
		private int? m_BranchId;
#if SIMPLSHARP
		public IEnumerable<CresnetBranch> Branches
		{
			get
			{
				CsaPws10sHubEnet master = m_Device as CsaPws10sHubEnet;
				if (master != null)
				{
					foreach (var branch in master.Branches.Cast<CresnetBranch>())
					{
						yield return branch;
					}
				}
			}
		}


		private void SetDevice(CsaPws10sHubEnetBase device, int? parentId, int? branchId)
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
		protected override void CopySettingsFinal(CsaPws10sHubEnetAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);
#if SIMPLSHARP
			if (m_Device is CsaPws10sHubEnet)
			{
				settings.Ipid = (byte)m_Device.ID;
			}
			else if (m_Device is CsaPws10sHubEnetSlave)
			{
				settings.CresnetId = (byte)m_Device.ID;
				settings.BranchId = m_BranchId;
				settings.ParentId = m_ParentId;
			}
#endif
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(CsaPws10sHubEnetAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);
#if SIMPLSHARP
			CsaPws10sHubEnetBase device = null;
			try
			{
				byte? id = settings.Ipid ?? settings.CresnetId;

				if (!id.HasValue)
				{
					string message = string.Format("{0} failed to instantiate {1}, settings requires either an IPID or CresnetID",
													this, typeof(CsaPws10sHubEnetBase).Name);
					Logger.AddEntry(eSeverity.Error, message);
					return;
				}

				device = CresnetUtils.InstantiateCresnetDevice<CsaPws10sHubEnetBase>(id.Value,
															   settings.BranchId,
															   settings.ParentId,
															   factory,
															   ipid => new CsaPws10sHubEnet(ipid, ProgramInfo.ControlSystem),
															   (cresnetId, branchId) => new CsaPws10sHubEnetSlave(cresnetId, branchId));
			}
			catch (ArgumentException e)
			{
				string message = string.Format("{0} failed to instantiate {1} with Cresnet ID {2} - {3}",
											   this, typeof(CsaPws10sHubEnet).Name, settings.CresnetId, e.Message);
				Logger.AddEntry(eSeverity.Error, e, message);
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