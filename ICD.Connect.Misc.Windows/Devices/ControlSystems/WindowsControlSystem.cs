using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.Misc.ControlSystems;
#if !SIMPLSHARP
using ICD.Connect.Misc.Windows.Utils;
#endif

namespace ICD.Connect.Misc.Windows.Devices.ControlSystems
{
	public sealed class WindowsControlSystem : AbstractControlSystemDevice<WindowsControlSystemSettings>
	{
		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return true;
		}

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

#if !SIMPLSHARP
			yield return new GenericConsoleCommand<string, string>("SwitchUser", "SwitchUser <USERNAME> <PASSWORD>",
			                                                       (u, p) => LogonUtils.SwitchUser(u, p));
#endif
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
