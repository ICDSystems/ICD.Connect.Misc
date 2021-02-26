using ICD.Connect.Devices;
using ICD.Connect.Telemetry.Attributes;

namespace ICD.Connect.Misc.ControlSystems
{
	[ExternalTelemetry("ControlSystemInfo", typeof(ControlSystemExternalTelemetryProvider))]
	public interface IControlSystemDevice : IDevice
	{
		/// <summary>
		/// Reboots the device.
		/// </summary>
		[MethodTelemetry("Reboot")]
		void Reboot();
	}
}