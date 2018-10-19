using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Misc.Occupancy
{
	public sealed class MockOccupancySensorControl : AbstractOccupancySensorControl<MockOccupancySensorDevice>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public MockOccupancySensorControl(MockOccupancySensorDevice parent, int id)
			: base(parent, id)
		{
		}

		#region Methods

		public void SetOccupied()
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

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Occupancy State", OccupancyState);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand c in GetBaseConsoleCommands())
				yield return c;

			yield return new ConsoleCommand("SetOccupied", "Sets Sensor to Occupied", () => SetOccupied());
			yield return new ConsoleCommand("SetUnoccupied", "Sets the Sensor to Unoccupied", () => SetUnoccupied());
			yield return new ConsoleCommand("SetUnknown", "Sets the Sensor to Unknown", () => SetUnknown());
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
