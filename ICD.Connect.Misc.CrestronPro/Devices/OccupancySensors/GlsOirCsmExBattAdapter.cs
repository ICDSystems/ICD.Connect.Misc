using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Misc.CrestronPro.InfinetEx;
using ICD.Connect.Partitioning.Commercial.Controls.Occupancy;
using ICD.Connect.Partitioning.Commercial.Devices.Occupancy;
using ICD.Connect.Settings;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
#if !NETSTANDARD
	public sealed class GlsOirCsmExBattAdapter : AbstractInfinetExAdapter<GlsOirCsmExBatt, GlsOirCsmExBattAdapterSettings>,
	                                             IOccupancySensorDevice
#else
	public sealed class GlsOirCsmExBattAdapter : AbstractInfinetExAdapter<GlsOirCsmExBattAdapterSettings>, IOccupancySensorDevice
#endif
	{

		#region Fields

		private eOccupancyState m_OccupancyState;

		#endregion

		#region Events

		public event EventHandler<GenericEventArgs<eOccupancyState>> OnOccupancyStateChanged;

		#endregion

		#region Properties

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

		private void UpdateOccupancyStatus()
		{
#if !NETSTANDARD
			OccupancyState = Device == null
				                 ? eOccupancyState.Unknown
				                 : Device.OccupancyDetectedFeedback.BoolValue
					                   ? eOccupancyState.Occupied
					                   : eOccupancyState.Unoccupied;
#endif
		}

		#region Sensor Callbacks

#if !NETSTANDARD

		protected override GlsOirCsmExBatt InstantiateDevice(byte rfid, GatewayBase gateway)
		{
			return new GlsOirCsmExBatt(rfid, gateway);
		}

		protected override void Subscribe(GlsOirCsmExBatt sensor)
		{
			base.Subscribe(sensor);

			if (sensor == null)
				return;

			sensor.BaseEvent += SensorOnBaseEvent;
		}

		protected override void Unsubscribe(GlsOirCsmExBatt sensor)
		{
			base.Unsubscribe(sensor);

			if (sensor == null)
				return;

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

#endif

		#endregion

		protected override void AddControls(GlsOirCsmExBattAdapterSettings settings, IDeviceFactory factory,
		                                    Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new OccupancySensorControl(this, 0));
		}
	}
}