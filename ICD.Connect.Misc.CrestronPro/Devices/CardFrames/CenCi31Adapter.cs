#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.ThreeSeriesCards;
#else
using System;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.CardFrames
{
#if !NETSTANDARD
	public sealed class CenCi31Adapter : AbstractCardFrameDevice<CenCi31, CenCi31AdapterSettings>
	{
		/// <summary>
		/// Creates a new card frame with the given parameters.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="controlSystem"></param>
		/// <returns></returns>
		protected override CenCi31 Instantiate(byte ipid, CrestronControlSystem controlSystem)
		{
			return new CenCi31(ipid, controlSystem);
		}
	}
#else
	public sealed class CenCi31Adapter : AbstractCardFrameDevice<CenCi31AdapterSettings>
	{
	}
#endif
}
