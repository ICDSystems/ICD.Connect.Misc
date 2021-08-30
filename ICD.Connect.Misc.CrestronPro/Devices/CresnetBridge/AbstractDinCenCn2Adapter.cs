using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.GeneralIO;
#endif
using System.Collections.Generic;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
#if !NETSTANDARD
	public abstract class AbstractDinCenCn2Adapter<TBridge, TSettings> :
		AbstractDevice<TSettings>, ICresnetBridgeAdapter
		where TSettings : ICresnetBridgeAdapterSettings, new()
		where TBridge : DinCenCn2
#else
	public abstract class AbstractDinCenCn2Adapter<TSettings> :
		AbstractDevice<TSettings>
		where TSettings : IDeviceSettings, new()
#endif
	{

#if !NETSTANDARD
		private byte m_Ipid;

		protected abstract TBridge InstantiateBridge(byte ipid);

		protected TBridge Bridge { get; private set; }

		private void SetBridge(TBridge bridge)
		{
			if (bridge == Bridge)
				return;

			Unsubscribe(Bridge);

			if (Bridge != null)
				GenericBaseUtils.TearDown(Bridge);

			Bridge = bridge;

			eDeviceRegistrationUnRegistrationResponse result;
			if (Bridge != null && !GenericBaseUtils.SetUp(Bridge, this, out result))
				Logger.Log(eSeverity.Error, "Unable to register {0} - {1}", Bridge.GetType().Name, result);

			Subscribe(Bridge);

			UpdateCachedOnlineStatus();
		}

		public virtual IEnumerable<CresnetBranch> Branches
		{
			get { return Bridge.Branches; }
		}

		#region Bridge Callbacks      

		private void Subscribe(TBridge bridge)
		{
			if (bridge == null)
				return;

			bridge.OnlineStatusChange += BridgeOnlineStatusChange;
		}

		private void Unsubscribe(TBridge bridge)
		{
			if (bridge == null)
				return;

			bridge.OnlineStatusChange -= BridgeOnlineStatusChange;
		}

		private void BridgeOnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

#endif
		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if !NETSTANDARD
			return Bridge != null && Bridge.IsOnline;
#else
			return false;
#endif
		}


		#region Settings

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);
#if !NETSTANDARD
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
#if !NETSTANDARD
			settings.Ipid = m_Ipid;
#endif
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();
#if !NETSTANDARD
			SetBridge(null);
#endif
		}

		#endregion
	}
}