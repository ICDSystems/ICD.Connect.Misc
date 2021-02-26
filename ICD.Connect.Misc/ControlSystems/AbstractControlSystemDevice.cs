using ICD.Connect.Devices;

namespace ICD.Connect.Misc.ControlSystems
{
	public abstract class AbstractControlSystemDevice<TSettings> : AbstractDevice<TSettings>, IControlSystemDevice
		where TSettings : IControlSystemDeviceSettings, new()
	{
	}
}
