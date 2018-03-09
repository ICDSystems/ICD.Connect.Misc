#if SIMPLSHARP
using Crestron.SimplSharpPro.ThreeSeriesCards;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
#if SIMPLSHARP
	public sealed class C3Ry8Adapter : AbstractC3RyAdapter<C3ry8, C3Ry8AdapterSettings>
	{
		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3ry8 InstantiateCard(CenCi31 cardFrame)
		{
			return new C3ry8(cardFrame);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3ry8 InstantiateCard(byte ipid, CenCi33 cardFrame)
		{
			return new C3ry8(ipid, cardFrame);
		}
	}
#else
    public sealed class C3Ry8Adapter : AbstractC3RyAdapter<C3Ry8AdapterSettings>
    {
    }
#endif
}
