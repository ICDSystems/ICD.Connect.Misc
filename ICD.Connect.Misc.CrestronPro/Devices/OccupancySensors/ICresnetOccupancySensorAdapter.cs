using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Misc.Occupancy;

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
	public interface ICresnetOccupancySensorAdapter: IDevice
	{

		event EventHandler<GenericEventArgs<eOccupancyState>> OnOccupancyStateChanged;

		eOccupancyState OccupancyState { get; }
	}
}