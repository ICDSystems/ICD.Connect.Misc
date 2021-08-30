using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Partitioning.Commercial.Controls.Occupancy;
using ICD.Connect.Partitioning.Commercial.Devices.Occupancy;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
using ICD.Connect.Misc.CrestronPro.Utils;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
#if !NETSTANDARD
	public sealed class CenOdtCPoeAdapter : AbstractDevice<CenOdtCPoeAdapterSettings>, IOccupancySensorDevice
#else
	public sealed class CenOdtCPoeAdapter : AbstractDevice<CenOdtCPoeAdapterSettings>, IOccupancySensorDevice
#endif
	{
		#region Events

		public event EventHandler<GenericEventArgs<eOccupancyState>> OnOccupancyStateChanged;

		#endregion

		#region Fields

#if !NETSTANDARD
		private CenOdtCPoe m_Sensor;
#endif

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
#if !NETSTANDARD
			return m_Sensor != null && m_Sensor.IsOnline;
#else
			return false;
#endif
		}

#if !NETSTANDARD
		/// <summary>
		/// Sets the wrapped device.
		/// </summary>
		/// <param name="device"></param>
		public void SetDevice(CenOdtCPoe device)
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
#if !NETSTANDARD

			if (m_Sensor == null)
			{
				OccupancyState = eOccupancyState.Unknown;
				return;
			}

			if (m_Sensor.OccupancyDetectedFeedback.BoolValue)
				OccupancyState = eOccupancyState.Occupied;
			else if (m_Sensor.VacancyDetectedFeedback.BoolValue)
				OccupancyState = eOccupancyState.Unoccupied;
			else
				OccupancyState = eOccupancyState.Unknown;
#else
			OccupancyState = eOccupancyState.Unknown;
#endif
		}

		#region Sensor Callbacks

#if !NETSTANDARD
		private void Subscribe(CenOdtCPoe sensor)
		{
			if (sensor == null)
				return;

			sensor.OnlineStatusChange += SensorOnLineStatusChange;
			sensor.BaseEvent += SensorOnBaseEvent;
		}

		private void Unsubscribe(CenOdtCPoe sensor)
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
		protected override void ApplySettingsFinal(CenOdtCPoeAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

#if !NETSTANDARD
			if (settings.Ipid == null || !CresnetUtils.IsValidId(settings.Ipid.Value))
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} - Ipid {1} is out of range",
						   typeof(CenOdtCPoe).Name, settings.Ipid);
				return;
			}

			CenOdtCPoe device = null;

			try
			{
				device = new CenOdtCPoe(settings.Ipid.Value, ProgramInfo.ControlSystem);

			}
			catch (ArgumentException e)
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} with Cresnet ID {1} - {2}",
						   typeof(CenOdtCPoe).Name, settings.Ipid, e.Message);
			}

			SetDevice(device);
#else
			throw new NotSupportedException();
#endif
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(CenOdtCPoeAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);
#if !NETSTANDARD
			settings.Ipid = m_Sensor == null ? null : (byte?)m_Sensor.ID;
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
#if !NETSTANDARD
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
		protected override void AddControls(CenOdtCPoeAdapterSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new OccupancySensorControl(this, 0));
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

#if !NETSTANDARD
			addRow("IPID", m_Sensor == null ? (uint?)null : m_Sensor.ID);
			addRow("Occupancy State", OccupancyState);
#endif
		}

		#endregion
	}
}