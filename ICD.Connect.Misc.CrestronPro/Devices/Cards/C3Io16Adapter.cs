using System.Collections.Generic;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.ThreeSeriesCards;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
#if SIMPLSHARP
    public sealed class C3Io16Adapter : AbstractC3CardAdapter<C3io16, C3Io16AdapterSettings>
	{
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public override Versiport GetIoPort(int address)
		{
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(Versiport).Name, address);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3io16 InstantiateCard(CenCi31 cardFrame)
		{
			return new C3io16(cardFrame);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3io16 InstantiateCard(byte ipid, CenCi33 cardFrame)
		{
			return new C3io16(ipid, cardFrame);
		}
	}
#else
    public sealed class C3Io16Adapter : AbstractC3CardAdapter<C3Io16AdapterSettings>
    {
    }
#endif
}
