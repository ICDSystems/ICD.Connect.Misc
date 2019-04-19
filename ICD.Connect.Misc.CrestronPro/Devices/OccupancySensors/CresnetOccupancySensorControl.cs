using ICD.Common.Utils.EventArguments;
using ICD.Connect.Misc.Occupancy;

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
	public sealed class CresnetOccupancySensorControl : AbstractOccupancySensorControl<ICresnetOccupancySensorAdapter>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public CresnetOccupancySensorControl(ICresnetOccupancySensorAdapter parent, int id) : base(parent, id)
		{
			parent.OnOccupancyStateChanged += ParentOnOccupancyStateChanged;
		}

		private void ParentOnOccupancyStateChanged(object sender, GenericEventArgs<eOccupancyState> args)
		{
			OccupancyState = args.Data;
		}
	}
}