using ICD.Connect.Devices;
using ICD.Connect.Misc.GlobalCache.FlexApi;
using ICD.Connect.Protocol.Network.Tcp;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	public interface IGcITachDevice : IDevice
	{
		/// <summary>
		/// Gets the network address of the device.
		/// </summary>
		string Address { get; }

		/// <summary>
		/// Sets the TCP client for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		void SetPort(AsyncTcpClient port);

		/// <summary>
		/// Sends the command to the device.
		/// </summary>
		/// <param name="command"></param>
		void SendCommand(string command);

		/// <summary>
		/// Sends the command to the device.
		/// </summary>
		/// <param name="command"></param>
		void SendCommand(FlexData command);
	}
}