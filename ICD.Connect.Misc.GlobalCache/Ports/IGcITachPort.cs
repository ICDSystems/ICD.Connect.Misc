using ICD.Connect.Misc.GlobalCache.Devices;
using ICD.Connect.Protocol.Ports;

namespace ICD.Connect.Misc.GlobalCache.Ports
{
	public interface IGcITachPort : IPort
	{
		IGcITachDevice Device { get; }

		int Module { get; }

		int Address { get; }
	}
}
