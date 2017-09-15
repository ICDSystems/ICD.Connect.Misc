#if SIMPLSHARP
using Crestron.SimplSharpPro.DM;
#endif
using ICD.Common.Properties;

namespace ICD.Connect.Misc.CrestronPro.Devices
{
	/// <summary>
	/// A IDmParent provides methods for getting attached DM devices.
	/// </summary>
	[PublicAPI]
	public interface IDmParent
	{
#if SIMPLSHARP
		/// <summary>
		/// Gets the DMInput at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		[PublicAPI]
		DMInput GetDmInput(int address);

		/// <summary>
		/// Gets the DMOutput at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		[PublicAPI]
		DMOutput GetDmOutput(int address);
#endif
	}
}
