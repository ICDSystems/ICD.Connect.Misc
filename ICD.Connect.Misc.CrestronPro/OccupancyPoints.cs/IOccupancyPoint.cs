using ICD.Common.Properties;
using ICD.Connect.Devices.Points;
using ICD.Connect.Misc.Occupancy;

namespace ICD.Connect.Misc.CrestronPro.OccupancyPoints.cs
{
	public interface IOccupancyPoint : IPoint
	{
		/// <summary>
		/// Gets the control for this point.
		/// </summary>
		[CanBeNull]
		new IOccupancySensorControl Control { get; }
	}
}