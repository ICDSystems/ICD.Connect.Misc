using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.ControlSystems
{
	public abstract class AbstractControlSystemDevice<TSettings> : AbstractDevice<TSettings>, IControlSystemDevice
		where TSettings : IControlSystemDeviceSettings, new()
	{
		private readonly ControlSystemDeviceTelemetryComponent m_TelemetryComponent;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractControlSystemDevice()
		{
			m_TelemetryComponent = new ControlSystemDeviceTelemetryComponent(this);
		}

		/// <summary>
		/// Reboots the device.
		/// </summary>
		public void Reboot()
		{
			ProcessorUtils.Reboot();
		}

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			foreach (IConsoleNodeBase node in ControlSystemDeviceConsole.GetConsoleNodes(this))
				yield return node;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			ControlSystemDeviceConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in ControlSystemDeviceConsole.GetConsoleCommands(this))
				yield return command;
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
