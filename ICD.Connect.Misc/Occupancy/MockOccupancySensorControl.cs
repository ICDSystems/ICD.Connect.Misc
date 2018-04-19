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
		    SetOccupancyState(eOccupancyState.Occupied);
	    }

	    public void SetUnoccupied()
	    {
		    SetOccupancyState(eOccupancyState.Unoccupied);
	    }

	    public void SetUnknown()
	    {
		    SetOccupancyState(eOccupancyState.Unknown);
	    }

	    private void SetOccupancyState(eOccupancyState state)
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
		    addRow("State", OccupancyState);
	    }

	    public override IEnumerable<IConsoleCommand> GetConsoleCommands()
	    {
			foreach (IConsoleCommand c in GetBaseConsoleCommands())
				yield return c;

			yield return new ConsoleCommand("SetOccupied", "Sets Sensor to Occupied", () => SetOccupided());
			yield return new ConsoleCommand("SetUnoccupied", "Sets the Sensor to Unoccupied", () => SetUnoccupied());
			yield return new ConsoleCommand("SetUnknown", "Sets the Sensor to Unknown", () => SetUnknown());

	    }

	    private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
	    {
		    return base.GetConsoleCommands();
	    }

	    #endregion

    }
}
