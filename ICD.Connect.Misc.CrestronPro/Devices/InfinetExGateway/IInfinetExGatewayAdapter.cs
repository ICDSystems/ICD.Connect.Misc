using ICD.Common.Properties;
using ICD.Connect.Devices;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.InfinetExGateway
{
	public interface IInfinetExGatewayAdapter : IDevice
	{
#if !NETSTANDARD
		[CanBeNull]
		GatewayBase InfinetExGateway { get; }
#endif
	}
}