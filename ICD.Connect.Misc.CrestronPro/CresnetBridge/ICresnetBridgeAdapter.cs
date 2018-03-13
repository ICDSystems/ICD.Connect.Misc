using System.Collections.Generic;
using ICD.Connect.Devices;
#if SIMPLSHARP
using Crestron.SimplSharpPro.DeviceSupport;
#endif

namespace ICD.Connect.Misc.CrestronPro.CresnetBridge
{
	public interface ICresnetBridgeAdapter : IDevice
	{
#if SIMPLSHARP
		 IEnumerable<CresnetBranch> Branches { get; }
#endif
	}
}