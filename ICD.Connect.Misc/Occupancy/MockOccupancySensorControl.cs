using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.Partitioning.Commercial.Controls.Occupancy;

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

		public void SetPeopleCount(int count)
		{
			PeopleCount = count;
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand c in GetBaseConsoleCommands())
				yield return c;

			yield return new GenericConsoleCommand<bool>("SetOccupancySupported","Sets occupancy support on the sensor", b => SetOccupancySupported(b));
			yield return new GenericConsoleCommand<bool>("SetPeopleCountSupported", "Sets people count support on the sensor", b => SetPeopleCountSupported(b));
			yield return new ConsoleCommand("SetOccupied", "Sets Sensor to Occupied", () => SetOccupied());
			yield return new ConsoleCommand("SetUnoccupied", "Sets the Sensor to Unoccupied", () => SetUnoccupied());
			yield return new ConsoleCommand("SetUnknown", "Sets the Sensor to Unknown", () => SetUnknown());
			yield return new GenericConsoleCommand<int>("SetPeopleCount", "Sets the people counted", i => SetPeopleCount(i));
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
