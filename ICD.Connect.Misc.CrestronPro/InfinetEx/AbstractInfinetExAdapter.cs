using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
#endif

namespace ICD.Connect.Misc.CrestronPro.InfinetEx
{
#if !NETSTANDARD
	public abstract class AbstractInfinetExAdapter<TDevice, TSettings> : AbstractDevice<TSettings>, IInfinetExDevice
		where TSettings : IInfinetExDeviceSettings, new()
		where TDevice : GenericDevice
#else
	public abstract class AbstractInfinetExAdapter<TSettings>:AbstractDevice<TSettings>, IInfinetExDevice
		where TSettings : IInfinetExDeviceSettings, new()
#endif
	{

		private readonly InfinetExInfo m_InfinetExInfo;

#if !NETSTANDARD
		private TDevice m_Device;

		public TDevice Device { get { return m_Device; } }
#endif

		public InfinetExInfo InfinetExInfo { get { return m_InfinetExInfo; } }

		protected AbstractInfinetExAdapter()
		{
			m_InfinetExInfo = new InfinetExInfo();
		}

		protected override bool GetIsOnlineStatus()
		{
#if !NETSTANDARD
			return Device != null && Device.IsOnline;
#else
			return false;
#endif
		}

		#region Device Callbacks

#if !NETSTANDARD

		private void SetDevice(TDevice device)
		{
			Unsubscribe(m_Device);

			m_Device = device;

			Subscribe(m_Device);

			UpdateDevice();
		}

		/// <summary>
		/// Updates the device state after being set
		/// </summary>
		protected virtual void UpdateDevice()
		{
			UpdateCachedOnlineStatus();
		}

		protected virtual void Subscribe(TDevice device)
		{
			if (device == null)
				return;

			device.OnlineStatusChange += DeviceOnLineStatusChange;
		}

		protected virtual void Unsubscribe(TDevice device)
		{
			if (device == null)
				return;

			device.OnlineStatusChange -= DeviceOnLineStatusChange;
		}

		private void DeviceOnLineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

#endif

		#endregion

		#region Settings

#if !NETSTANDARD
		protected abstract TDevice InstantiateDevice(byte rfid, GatewayBase gateway);
#endif

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			InfinetExInfo.ApplySettings(settings);

#if !NETSTANDARD

			if (!InfinetExInfo.RfId.HasValue || !InfinetExUtils.IsValidId(InfinetExInfo.RfId.Value))
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} - RfId {1} is out of range",
						   typeof(TDevice).Name,
						   InfinetExInfo.RfId.HasValue
							   ? StringUtils.ToIpIdString(InfinetExInfo.RfId.Value)
							   : null);
				return;
			}

			if (!InfinetExInfo.ParentId.HasValue)
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} - no ParentId defined",
						   typeof(TDevice).Name);
				return;
			}

			TDevice device = InfinetExUtils.InstantiateInfinetExDevice(InfinetExInfo.RfId.Value, InfinetExInfo.ParentId.Value, factory,
																			   (rfid, gateway) =>InstantiateDevice(rfid, gateway));

			SetDevice(device);
#else
			throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			InfinetExInfo.CopySettings(settings);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			InfinetExInfo.ClearSettings();
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

			InfinetExDeviceConsole.BuildConsoleStatus(this, addRow);
		}

		#endregion
	}
}