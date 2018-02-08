﻿using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Misc.Occupancy
{
	public abstract class AbstractOccupancySensorControl<T> : AbstractDeviceControl<T>, IOccupancySensor where T : IDeviceBase
	{

		#region fields

		private bool m_OccupancyState;

		#endregion

		#region events

		public event EventHandler<BoolEventArgs> OnOccupancyStateChanged;

		#endregion

		#region properties

		/// <summary>
		/// State of the occupancy sensor
		/// True = occupied
		/// False = unoccupied/vacant
		/// </summary>
		public bool OccupancyState
		{
			get { return m_OccupancyState; }
			protected set
			{
				if (m_OccupancyState == value)
					return;
				m_OccupancyState = value;
				OnOccupancyStateChanged.Raise(this, new BoolEventArgs(value));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		protected AbstractOccupancySensorControl(T parent, int id) : base(parent, id)
		{
		}

		
	}
}