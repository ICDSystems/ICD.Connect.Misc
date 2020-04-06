using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices.Mock;

namespace ICD.Connect.Misc.Occupancy
{
	public sealed class MockOccupancySensorDevice : AbstractMockDevice<MockOccupancySensorDeviceSettings>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public MockOccupancySensorDevice()
		{
			Controls.Add(new MockOccupancySensorControl(this, 0));
		}

		#region Methods

		/// <summary>
		/// Adds a control with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool AddOccupancySensorControl(int id)
		{
			if (Controls.Contains(id))
				return false;

			Controls.Add(new MockOccupancySensorControl(this, id));

			return true;
		}

		/// <summary>
		/// Removes the control with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[PublicAPI]
		public bool RemoveOccupancySensorControl(int id)
		{
			return Controls.Remove(id);
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return
				new GenericConsoleCommand<int>("AddOccupancySensorControl", "AddOccupancySensorControl <ID>",
				                               id => AddOccupancySensorControl(id));
			yield return
				new GenericConsoleCommand<int>("RemoveOccupancySensorControl", "RemoveOccupancySensorControl <ID>",
				                               id => RemoveOccupancySensorControl(id));
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
