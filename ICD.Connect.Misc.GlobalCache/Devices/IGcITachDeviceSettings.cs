using ICD.Connect.Devices;
using ICD.Connect.Protocol.Network.Ports.Tcp;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	public interface IGcITachDeviceSettings : IDeviceSettings, INetworkSettings
	{
		/// <summary>
		/// The port id.
		/// </summary>
		[OriginatorIdSettingsProperty(typeof(IcdTcpClient))]
		int? Port { get; set; }
	}
}
