#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
#endif
using ICD.Common.Properties;

namespace ICD.Connect.Misc.CrestronPro.Devices
{
	public enum eInputOuptut
	{
		Input,
		Output
	}

	/// <summary>
	/// IPortParent simply provides methods for accessing the hardware ports on a device.
	/// </summary>
	[PublicAPI]
	public interface IPortParent
	{
#if !NETSTANDARD
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

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		[PublicAPI]
		DigitalInput GetDigitalInputPort(int address);

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="io"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		Cec GetCecPort(eInputOuptut io, int address);
#endif
	}
}
