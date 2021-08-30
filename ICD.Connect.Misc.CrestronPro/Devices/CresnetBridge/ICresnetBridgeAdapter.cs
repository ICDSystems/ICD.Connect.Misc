using System.Collections.Generic;
using ICD.Connect.Devices;
#if !NETSTANDARD
using Crestron.SimplSharpPro.DeviceSupport;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
	public interface ICresnetBridgeAdapter : IDevice
	{
#if !NETSTANDARD
		IEnumerable<CresnetBranch> Branches { get; }
#endif
	}
}