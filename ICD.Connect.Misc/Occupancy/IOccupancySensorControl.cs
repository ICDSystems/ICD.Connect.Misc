using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Misc.Occupancy
{
	public enum eOccupancyState
	{
		Unknown,
		Unoccupied,
		Occupied
	}

	public interface IOccupancySensorControl : IDeviceControl
	{
		/// <summary>
		/// Triggered when the occupancy state changes
		/// True = occupied
		/// False = unoccupied/vacant
		/// </summary>
		event EventHandler<GenericEventArgs<eOccupancyState>> OnOccupancyStateChanged;

		/// <summary>
		/// State of the occupancy sensor
		/// True = occupied
		/// False = unoccupied/vacant
		/// </summary>
		eOccupancyState OccupancyState { get; }
	}
}
