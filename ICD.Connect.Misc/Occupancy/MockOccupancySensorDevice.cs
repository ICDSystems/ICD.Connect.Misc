using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Mock;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.Occupancy
{
	public sealed class MockOccupancySensorDevice : AbstractMockDevice<MockOccupancySensorDeviceSettings>
	{
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

		#region Settings

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(MockOccupancySensorDeviceSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new MockOccupancySensorControl(this, 0));
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
