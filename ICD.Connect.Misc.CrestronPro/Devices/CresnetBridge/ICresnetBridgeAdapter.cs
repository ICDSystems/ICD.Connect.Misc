using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using ICD.Connect.Devices;
#if SIMPLSHARP

#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
	public interface ICresnetBridgeAdapter : IDevice
	{
#if SIMPLSHARP
		 IEnumerable<CresnetBranch> Branches { get; }
#endif
	}
}