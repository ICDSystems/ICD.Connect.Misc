using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Misc.Occupancy
{
	public abstract class AbstractOccupancySensorControl<T> : AbstractDeviceControl<T>, IOccupancySensorControl
		where T : IDeviceBase
	{
		private eOccupancyState m_OccupancyState;

		#region events

		/// <summary>
		/// Triggered when the occupancy state changes
		/// True = occupied
		/// False = unoccupied/vacant
		/// </summary>
		public event EventHandler<GenericEventArgs<eOccupancyState>> OnOccupancyStateChanged;

		#endregion

		#region properties

		/// <summary>
		/// State of the occupancy sensor
		/// True = occupied
		/// False = unoccupied/vacant
		/// </summary>
		public eOccupancyState OccupancyState
		{
			get { return m_OccupancyState; }
			protected set
			{
				if (m_OccupancyState == value)
					return;

				m_OccupancyState = value;

				OnOccupancyStateChanged.Raise(this, new GenericEventArgs<eOccupancyState>(value));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		protected AbstractOccupancySensorControl(T parent, int id)
			: base(parent, id)
		{
		}
	}
}
