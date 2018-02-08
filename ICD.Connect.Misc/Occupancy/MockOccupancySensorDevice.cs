using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.Occupancy
{
	public sealed class MockOccupancySensorDevice : AbstractDevice<MockOccupancySensorDeviceSettings>
	{
	    public MockOccupancySensorDevice()
	    {
		    this.Controls.Add(new MockOccupancySensorControl(this, 0));
	    }

		#region Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return true;
		}

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

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return
				new GenericConsoleCommand<int>("AddOccupancySensorControl", "AddOccupancySensorControl <ID>", id => AddOccupancySensorControl(id));
			yield return
				new GenericConsoleCommand<int>("RemoveOccupancySensorControl", "RemoveOccupancySensorControl <ID>", id => RemoveOccupancySensorControl(id));
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
