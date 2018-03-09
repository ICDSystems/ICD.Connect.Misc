#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.ThreeSeriesCards;
#else
using System;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.CardFrames
{
#if SIMPLSHARP
	public sealed class CenCi33Adapter : AbstractCardFrameDevice<CenCi33, CenCi33AdapterSettings>
	{
		/// <summary>
		/// Creates a new card frame with the given parameters.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="controlSystem"></param>
		/// <returns></returns>
		protected override CenCi33 Instantiate(byte ipid, CrestronControlSystem controlSystem)
		{
			return new CenCi33(ipid, controlSystem);
		}
	}
#else
	public sealed class CenCi33Adapter : AbstractCardFrameDevice<CenCi33AdapterSettings>
	{
	}
#endif
}
