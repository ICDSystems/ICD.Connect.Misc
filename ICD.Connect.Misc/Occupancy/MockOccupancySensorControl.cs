using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Misc.Occupancy
{
    public sealed class MockOccupancySensorControl : AbstractOccupancySensorControl<MockOccupancySensorDevice>
    {

		#region methods

	    public void SetOccupided()
	    {
		    SetOccupancyState(true);
	    }

	    public void SetUnoccupied()
	    {
		    SetOccupancyState(false);
	    }

	    private void SetOccupancyState(bool state)
	    {
		    OccupancyState = state;
	    }

		#endregion	

		public MockOccupancySensorControl(MockOccupancySensorDevice parent, int id) : base(parent, id)
	    {
	    }

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
	    {
		    base.BuildConsoleStatus(addRow);
		    addRow("Occupied", OccupancyState);
	    }

	    public override IEnumerable<IConsoleCommand> GetConsoleCommands()
	    {
			foreach (IConsoleCommand c in GetBaseConsoleCommands())
				yield return c;

			yield return new ConsoleCommand("SetOccupied", "Sets Sensor to Occupied", () => SetOccupided());
			yield return new ConsoleCommand("SetUnoccupied", "Sets the Sensor to Unoccupied", () => SetUnoccupied());

	    }

	    private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
	    {
		    return base.GetConsoleCommands();
	    }

	    #endregion

    }
}
