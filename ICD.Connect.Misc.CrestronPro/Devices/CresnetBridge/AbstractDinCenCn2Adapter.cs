#if SIMPLSHARP
#endif
using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.GeneralIO;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
#if SIMPLSHARP
	public abstract class AbstractDinCenCn2Adapter<TBridge, TSettings> :
		AbstractDevice<TSettings>, ICresnetBridgeAdapter
		where TSettings : ICresnetBridgeAdapterSettings, new()
		where TBridge : DinCenCn2
#else
	pubic abstract class AbstractDinCenCn2Adapter<TSettings> :
		AbstractDevice<TSettings>
		where TSettings : IDeviceSettings, new()
#endif
	{
		
#if SIMPLSHARP
		private byte m_Ipid;

		protected abstract TBridge InstantiateBridge(byte ipid);

		protected TBridge Bridge { get; private set; }

		private void SetBridge(TBridge bridge)
		{
			if (bridge == Bridge)
				return;

			if (Bridge != null)
			{
				if (Bridge.Registered)
					Bridge.UnRegister();

				try
				{
					Bridge.Dispose();
				}
				catch
				{
				}
			}

			Bridge = bridge;

			if (Bridge != null && !Bridge.Registered)
			{
				if (Name != null)
					Bridge.Description = Name;
				eDeviceRegistrationUnRegistrationResponse result = Bridge.Register();
				if (result != eDeviceRegistrationUnRegistrationResponse.Success)
					Logger.AddEntry(eSeverity.Error, "Unable to register {0} - {1}", Bridge.GetType().Name, result);
			}
			UpdateCachedOnlineStatus();
		}

		public virtual IEnumerable<CresnetBranch> Branches
		{
			get { return Bridge.Branches; }
		}
#endif

		#region Settings

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);
#if SIMPLSHARP
			if (settings.Ipid == null)
				return;
			m_Ipid = settings.Ipid.Value;
			TBridge bridge = InstantiateBridge(m_Ipid);
			SetBridge(bridge);
#endif
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);
#if SIMPLSHARP
			settings.Ipid = m_Ipid;
#endif
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();
#if SIMPLSHARP
			SetBridge(null);
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
			return Bridge != null && Bridge.IsOnline;
#else
            return false;
#endif
		}
	}
}