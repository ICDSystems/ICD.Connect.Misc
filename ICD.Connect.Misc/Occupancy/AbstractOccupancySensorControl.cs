using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Misc.Occupancy
{
	public abstract class AbstractOccupancySensorControl<T> : AbstractDeviceControl<T>, IOccupancySensorControl
		where T : IDevice
	{
		private eOccupancyState m_OccupancyState;

		#region events

		/// <summary>
		/// Triggered when the occupancy state changes
		/// True = occupied
		/// False = unoccupied/vacant
		/// </summary>
		public event EventHandler<GenericEventArgs<eOccupancyState>> OnOccupancyStateChanged;

		#endregion

		#region properties

		/// <summary>
		/// State of the occupancy sensor
		/// True = occupied
		/// False = unoccupied/vacant
		/// </summary>
		public eOccupancyState OccupancyState
		{
			get { return m_OccupancyState; }
			protected set
			{
				if (m_OccupancyState == value)
					return;

				m_OccupancyState = value;

				OnOccupancyStateChanged.Raise(this, new GenericEventArgs<eOccupancyState>(value));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		protected AbstractOccupancySensorControl(T parent, int id)
			: base(parent, id)
		{
		}

		#region Console

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in OccupancySensorControlConsole.GetConsoleCommands(this))
				yield return command;
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			foreach (IConsoleNodeBase node in OccupancySensorControlConsole.GetConsoleNodes(this))
				yield return node;
		}

		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			OccupancySensorControlConsole.BuildConsoleStatus(this, addRow);
		}

		#endregion
	}
}
