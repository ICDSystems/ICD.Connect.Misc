using ICD.Connect.Devices.Points;
using ICD.Connect.Misc.Occupancy;

namespace ICD.Connect.Misc.CrestronPro.OccupancyPoints.cs
{
	public abstract class AbstractOccupancyPoint<TSettings> : AbstractPoint<TSettings, IOccupancySensorControl>, IOccupancyPoint
		where TSettings : IOccupancyPointSettings, new()
	{
		/// <summary>
		/// Gets the category for this originator type (e.g. Device, Port, etc)
		/// </summary>
		public override string Category { get { return "OccupancyPoint"; } }

		/// <summary>
		/// Gets the control for this point.
		/// </summary>
		public IOccupancySensorControl Control { get; private set; }
	}
}