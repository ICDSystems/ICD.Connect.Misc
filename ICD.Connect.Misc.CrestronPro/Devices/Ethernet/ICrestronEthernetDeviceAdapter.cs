using ICD.Connect.Devices;
using ICD.Connect.Protocol.Network.Settings;

namespace ICD.Connect.Misc.CrestronPro.Devices.Ethernet
{
	public interface ICrestronEthernetDeviceAdapter : IDevice
	{
		SecureNetworkProperties NetworkProperties { get; }
	}
}