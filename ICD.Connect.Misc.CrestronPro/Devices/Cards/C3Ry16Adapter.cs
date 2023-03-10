#if !NETSTANDARD
using Crestron.SimplSharpPro.ThreeSeriesCards;
using Crestron.SimplSharpProInternal;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
#if !NETSTANDARD
	public sealed class C3Ry16Adapter : AbstractC3RyAdapter<C3ry16, C3Ry16AdapterSettings>
	{
		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3ry16 InstantiateCard(Ci3SingleCardCage cardFrame)
		{
			return new C3ry16(cardFrame);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardId"></param>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3ry16 InstantiateCard(uint cardId, Ci3MultiCardCage cardFrame)
		{
			return new C3ry16(cardId, cardFrame);
		}
	}
#else
    public sealed class C3Ry16Adapter : AbstractC3RyAdapter<C3Ry16AdapterSettings>
    {
    }
#endif
}
