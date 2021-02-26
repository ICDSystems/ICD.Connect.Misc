using ICD.Connect.Misc.ControlSystems;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.Windows.Devices.ControlSystems
{
	[KrangSettings("WindowsControlSystem", typeof(WindowsControlSystem))]
	public sealed class WindowsControlSystemSettings : AbstractControlSystemDeviceSettings
	{
	}
}
