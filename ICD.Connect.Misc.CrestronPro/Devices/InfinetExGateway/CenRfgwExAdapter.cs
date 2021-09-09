using System;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.Gateways;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.InfinetExGateway
{
	public sealed class CenRfgwExAdapter : AbstractDevice<CenRfgwExAdapterSettings>, IInfinetExGatewayAdapter
	{

#if !NETSTANDARD
		private CenRfgwExEthernetSharable m_Gateway;

		public GatewayBase InfinetExGateway { get { return m_Gateway; } }
#endif

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if !NETSTANDARD
			return m_Gateway != null && m_Gateway.IsOnline;
#else
			return false;
#endif
		}

#if !NETSTANDARD
		protected override void CopySettingsFinal(CenRfgwExAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Ipid = (byte?)(m_Gateway == null ? 0 : m_Gateway.ID);
		}

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetGateway(null);
		}
#endif

		protected override void ApplySettingsFinal(CenRfgwExAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

#if !NETSTANDARD
			CenRfgwExEthernetSharable gateway = null;

			try
			{
				if (settings.Ipid.HasValue)
					gateway = new CenRfgwExEthernetSharable(settings.Ipid.Value, ProgramInfo.ControlSystem);
			}
			catch (Exception e)
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} - {1}", typeof(CenRfgwExEthernetSharable).Name, e.Message);
			}

			SetGateway(gateway);
#else
			throw new NotSupportedException();
#endif
		}

		#region Gateway Callbacks

#if !NETSTANDARD

		/// <summary>
		/// Override to control how the switcher is assigned from settings.
		/// </summary>
		private void SetGateway(CenRfgwExEthernetSharable gateway)
		{
			Unsubscribe(m_Gateway);
			m_Gateway = gateway;
			Subscribe(m_Gateway);

			UpdateCachedOnlineStatus();
		}

		private void Subscribe(CenRfgwExEthernetSharable gateway)
		{
			if (gateway == null)
				return;

			gateway.OnlineStatusChange += GatewayOnLineStatusChange;
		}

		private void Unsubscribe(CenRfgwExEthernetSharable gateway)
		{
			if (gateway == null)
				return;

			gateway.OnlineStatusChange -= GatewayOnLineStatusChange;
		}

		private void GatewayOnLineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

#endif

		#endregion
	}
}