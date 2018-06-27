#if SIMPLSHARP
using Crestron.SimplSharpPro.ThreeSeriesCards;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
#if SIMPLSHARP
	public sealed class C3Io16Adapter : AbstractC3CardAdapter<C3io16, C3Io16AdapterSettings>
	{
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
		/// <param name="cardId"></param>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3io16 InstantiateCard(uint cardId, CenCi33 cardFrame)
		{
			return new C3io16(cardId, cardFrame);
		}
	}
#else
    public sealed class C3Io16Adapter : AbstractC3CardAdapter<C3Io16AdapterSettings>
    {
    }
#endif
}
