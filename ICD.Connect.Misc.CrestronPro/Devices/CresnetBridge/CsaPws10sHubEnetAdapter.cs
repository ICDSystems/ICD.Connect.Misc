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

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
	public sealed class CsaPws10sHubEnetAdapter : AbstractDevice<CsaPws10sHubEnetAdapterSettings>, ICsaPws10sHubEnetAdapter, ICresnetBridgeAdapter
	{
#if SIMPLSHARP
		private CsaPws10sHubEnet m_Device;
#endif

#if SIMPLSHARP
		public IEnumerable<CresnetBranch> Branches
		{
			get
			{
				if (m_Device != null)
				{
					foreach (var branch in m_Device.Branches.Cast<CresnetBranch>())
					{
						yield return branch;
					}
				}
			}
		}

		private void SetDevice(CsaPws10sHubEnet device)
		{
			if (device == m_Device)
				return;

			m_Device.UnRegister();

			m_Device = device;

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
			settings.Ipid = m_Device == null ? (byte?)null : (byte)m_Device.ID;
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
			CsaPws10sHubEnet device = null;
			try
			{
				if (settings.Ipid.HasValue)
					device = new CsaPws10sHubEnet(settings.Ipid.Value, ProgramInfo.ControlSystem);
				else
					Log(eSeverity.Error, "Failed to instantiate {0} - Settings requires an IPID", typeof(CsaPws10sHubEnetBase).Name);
			}
			catch (ArgumentException e)
			{
				string message = string.Format("{0} - Failed to instantiate {1} with IPID {2} - {3}",
											   this, typeof(CsaPws10sHubEnet).Name, settings.Ipid, e.Message);
				Logger.AddEntry(eSeverity.Error, e, message);
			}
			finally
			{
				SetDevice(device);
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