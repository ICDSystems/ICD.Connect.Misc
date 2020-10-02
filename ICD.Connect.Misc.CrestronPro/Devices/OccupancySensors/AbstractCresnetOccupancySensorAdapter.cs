using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Partitioning.Commercial.Controls.Occupancy;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.GeneralIO;
using ICD.Connect.Misc.CrestronPro.Utils;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
#if SIMPLSHARP
	public abstract class AbstractCresnetOccupancySensorAdapter<TSettings, TSensor> : AbstractDevice<TSettings>, ICresnetOccupancySensorAdapter
		where TSensor : GlsOccupancySensorBase
		where TSettings : AbstractCresnetOccupancySensorAdapterSettings, new()
#else
		public abstract class AbstractCresnetOccupancySensorAdapter<TSettings> : AbstractDevice<TSettings>, ICresnetOccupancySensorAdapter
		where TSettings : AbstractCresnetOccupancySensorAdapterSettings, new()
#endif
	{
		#region Events

		public event EventHandler<GenericEventArgs<eOccupancyState>> OnOccupancyStateChanged;

		#endregion

		#region Fields

#if SIMPLSHARP
		private TSensor m_Sensor;
#endif

		private int? m_CresnetBranchId;
		private int? m_CresnetParentId;

		private eOccupancyState m_OccupancyState;

		#endregion

		#region Properties

		public eOccupancyState OccupancyState
		{
			get { return m_OccupancyState; }
			private set
			{
				if (value == m_OccupancyState)
					return;

				m_OccupancyState = value;

				OnOccupancyStateChanged.Raise(this, new GenericEventArgs<eOccupancyState>(m_OccupancyState));
			}
		}

		#endregion

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

#if SIMPLSHARP
		/// <summary>
		/// Sets the wrapped device.
		/// </summary>
		/// <param name="device"></param>
		public void SetDevice(TSensor device)
		{
			if (device == m_Sensor)
				return;

			Unsubscribe(m_Sensor);

			if (m_Sensor != null)
				GenericBaseUtils.TearDown(m_Sensor);

			m_Sensor = device;

			eDeviceRegistrationUnRegistrationResponse result;
			if (m_Sensor != null && !GenericBaseUtils.SetUp(m_Sensor, this, out result))
				Logger.Log(eSeverity.Error, "Unable to register {0} - {1}", m_Sensor.GetType().Name, result);

			Subscribe(m_Sensor);
			UpdateCachedOnlineStatus();

			UpdateStatus();
		}
#endif

		private void UpdateStatus()
		{
#if SIMPLSHARP

			if (m_Sensor == null)
			{
				OccupancyState = eOccupancyState.Unknown;
				return;
			}

			if(m_Sensor.OccupancyDetectedFeedback.BoolValue)
				OccupancyState = eOccupancyState.Occupied;
			else if (m_Sensor.VacancyDetectedFeedback.BoolValue)
				OccupancyState = eOccupancyState.Unoccupied;
			else
				OccupancyState = eOccupancyState.Unknown;
#else
			OccupancyState = eOccupancyState.Unknown;
#endif
		}

		#region Instantiation

#if SIMPLSHARP

		protected abstract TSensor InstantiateControlSystem(byte cresnetId, CrestronControlSystem controlSystem);

		protected abstract TSensor InstantiateCresnetBranch(byte cresnetId, CresnetBranch cresnetBranch);

#endif

		#endregion

		#region Sensor Callbacks

#if SIMPLSHARP
		private void Subscribe(TSensor sensor)
		{
			if (sensor == null)
				return;

			sensor.OnlineStatusChange += SensorOnLineStatusChange;
			sensor.BaseEvent += SensorOnBaseEvent;
		}

		private void Unsubscribe(TSensor sensor)
		{
			if (sensor == null)
				return;

			sensor.OnlineStatusChange -= SensorOnLineStatusChange;
			sensor.BaseEvent -= SensorOnBaseEvent;
		}

		private void SensorOnLineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		private void SensorOnBaseEvent(GenericBase device, BaseEventArgs args)
		{
			switch (args.EventId)
			{
				case GlsOccupancySensorBase.RoomOccupiedFeedbackEventId:
				case GlsOccupancySensorBase.RoomVacantFeedbackEventId:
					UpdateStatus();
					break;
			}
		}

#endif

		#endregion

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
			if (settings.CresnetId == null || !CresnetUtils.IsValidId(settings.CresnetId.Value))
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} - CresnetId {1} is out of range",
				    typeof(TSensor).Name,
				    settings.CresnetId.HasValue
					    ? StringUtils.ToIpIdString(settings.CresnetId.Value)
					    : null);
				return;
			}

			TSensor device = null;

			try
			{
				device = CresnetUtils.InstantiateCresnetDevice(settings.CresnetId.Value,
															   settings.BranchId,
															   settings.ParentId,
															   factory,
															   cresnetId => InstantiateControlSystem(cresnetId, ProgramInfo.ControlSystem),
															   InstantiateCresnetBranch);

			}
			catch (ArgumentException e)
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} with Cresnet ID {1} - {2}",
				           typeof(TSensor).Name,
				           StringUtils.ToIpIdString(settings.CresnetId.Value),
				           e.Message);
			}

			m_CresnetBranchId = settings.BranchId;
			m_CresnetParentId = settings.ParentId;

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
#if SIMPLSHARP
			settings.CresnetId = m_Sensor == null ? null : (byte?)m_Sensor.ID;
			settings.ParentId = m_CresnetParentId;
			settings.BranchId = m_CresnetBranchId;
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
#if SIMPLSHARP
			m_CresnetParentId = null;
			m_CresnetBranchId = null;
			SetDevice(null);
#else
			throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(TSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new CresnetOccupancySensorControl(this, 0));
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

#if SIMPLSHARP
			addRow("Cresnet ID", m_Sensor == null ? null : StringUtils.ToIpIdString((byte)m_Sensor.ID));
			addRow("Parent ID", m_CresnetParentId);
			addRow("Branch ID", m_CresnetBranchId);
			addRow("Occupancy State", OccupancyState);
#endif
		}

		#endregion
	}
}