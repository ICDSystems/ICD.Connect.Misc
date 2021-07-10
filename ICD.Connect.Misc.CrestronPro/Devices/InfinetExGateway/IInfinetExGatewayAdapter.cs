using Crestron.SimplSharpPro;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro.Devices.InfinetExGateway
{
	public interface IInfinetExGatewayAdapter : IDevice
	{
#if SIMPLSHARP
		GatewayBase InfinetExGateway { get; }
#endif
	}
}