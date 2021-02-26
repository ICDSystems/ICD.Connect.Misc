using ICD.Connect.Misc.ControlSystems;

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
	}
}
