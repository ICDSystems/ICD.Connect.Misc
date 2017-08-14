#if SIMPLSHARP
using Crestron.SimplSharpPro;
using ICD.Common.Properties;

namespace ICD.Connect.Misc.CrestronPro.Devices
{
	/// <summary>
	/// IPortParent simply provides methods for accessing the hardware ports on a device.
	/// </summary>
	[PublicAPI]
	public interface IPortParent
	{
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		[PublicAPI]
		ComPort GetComPort(int address);

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		[PublicAPI]
		IROutputPort GetIrOutputPort(int address);

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		[PublicAPI]
		Relay GetRelayPort(int address);

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		[PublicAPI]
		Versiport GetIoPort(int address);
	}
}
#endif
