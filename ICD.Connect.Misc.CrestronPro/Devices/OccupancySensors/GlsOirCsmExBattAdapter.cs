using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Misc.CrestronPro.InfinetEx;
using ICD.Connect.Partitioning.Commercial.Controls.Occupancy;
using ICD.Connect.Partitioning.Commercial.Devices.Occupancy;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
	public sealed class GlsOirCsmExBattAdapter : AbstractDevice<GlsOirCsmExBattAdapterSettings>, IInfinetExDevice, IOccupancySensorDevice
	{
#region Fields

		private readonly InfinetExInfo m_InfinetExInfo;


#if SIMPLSHARP
		private GlsOirCsmExBatt m_Sensor;
#endif

		private eOccupancyState m_OccupancyState;

#endregion

#region Events

		public event EventHandler<GenericEventArgs<eOccupancyState>> OnOccupancyStateChanged;

#endregion


#region Properties

		public InfinetExInfo InfinetExInfo { get { return m_InfinetExInfo; } }

		public eOccupancyState OccupancyState
		{
			get { return m_OccupancyState; }
			private set
			{
				if (m_OccupancyState == value)
					return;

				m_OccupancyState = value;

				OnOccupancyStateChanged.Raise(this, value);
			}
		}

#endregion

		public GlsOirCsmExBattAdapter()
		{
			m_InfinetExInfo = new InfinetExInfo();
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if SIMPLSHARP
			return m_Sensor != null && m_Sensor.IsOnline;
#else
			return false;
#endif
		}

		private void UpdateOccupancyStatus()
		{
#if SIMPLSHARP
			OccupancyState = m_Sensor == null
				                 ? eOccupancyState.Unknown
				                 : m_Sensor.OccupancyDetectedFeedback.BoolValue
					                   ? eOccupancyState.Occupied
					                   : eOccupancyState.Unoccupied;
#endif
		}

#region Sensor Callbacks

#if SIMPLSHARP

		private void SetSensor(GlsOirCsmExBatt sensor)
		{
			Unsubscribe(m_Sensor);

			m_Sensor = sensor;

			Subscribe(m_Sensor);
			
			UpdateCachedOnlineStatus();
			UpdateOccupancyStatus();
		}

		private void Subscribe(GlsOirCsmExBatt sensor)
		{
			if (sensor == null)
				return;

			sensor.OnlineStatusChange += SensorOnLineStatusChange;
			sensor.BaseEvent += SensorOnBaseEvent;
		}

		private void Unsubscribe(GlsOirCsmExBatt sensor)
		{
			if (sensor == null)
				return;

			sensor.OnlineStatusChange -= SensorOnLineStatusChange;
			sensor.BaseEvent -= SensorOnBaseEvent;
		}

		private void SensorOnBaseEvent(GenericBase device, BaseEventArgs args)
		{
			switch (args.EventId)
			{
				case GlsOirCsmExBattEventIds.OccupancyStatusReceivedEventId:
				case GlsOirCsmExBattEventIds.GraceOccupancyStatusReceivedEventId:
				case GlsOirCsmExBattEventIds.VacancyStatusReceived:
					UpdateOccupancyStatus();
					break;
			}
		}

		private void SensorOnLineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

#endif

#endregion


		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(GlsOirCsmExBattAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			InfinetExInfo.ApplySettings(settings);

#if SIMPLSHARP

			if (!InfinetExInfo.RfId.HasValue || !InfinetExUtils.IsValidId(InfinetExInfo.RfId.Value))
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} - RfId {1} is out of range",
				           typeof(GlsOirCsmExBatt).Name,
				           InfinetExInfo.RfId.HasValue
					           ? StringUtils.ToIpIdString(InfinetExInfo.RfId.Value)
					           : null);
				return;
			}

			if (!InfinetExInfo.ParentId.HasValue)
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} - no ParentId defined",
				           typeof(GlsOirCsmExBatt).Name);
				return;
			}

			GlsOirCsmExBatt sensor = InfinetExUtils.InstantiateInfinetExDevice(InfinetExInfo.RfId.Value, InfinetExInfo.ParentId.Value, factory,
			                                                                   (rfid, gateway) => new GlsOirCsmExBatt(rfid, gateway));

			SetSensor(sensor);
#else
			throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			InfinetExInfo.ClearSettings();
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(GlsOirCsmExBattAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			InfinetExInfo.CopySettings(settings);
		}

		protected override void AddControls(GlsOirCsmExBattAdapterSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new OccupancySensorControl(this, 0));
		}
	}
}