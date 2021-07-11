using ICD.Common.Properties;
using ICD.Connect.Devices;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.InfinetExGateway
{
	public interface IInfinetExGatewayAdapter : IDevice
	{
#if SIMPLSHARP
		[CanBeNull]
		GatewayBase InfinetExGateway { get; }
#endif
	}
}