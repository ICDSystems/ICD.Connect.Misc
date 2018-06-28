using System;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.ThreeSeriesCards;
using Crestron.SimplSharpProInternal;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
#if SIMPLSHARP
	public sealed class C3Ir8Adapter : AbstractC3CardAdapter<C3ir8, C3Ir8AdapterSettings>
	{
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public override IROutputPort GetIrOutputPort(int address)
		{
			if (Card.IROutputPorts.Contains((uint)address))
				return Card.IROutputPorts[(uint)address];

			string message = string.Format("{0} has no {1} with address {2}", this, typeof(IROutputPort).Name, address);
			throw new IndexOutOfRangeException(message);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3ir8 InstantiateCard(Ci3SingleCardCage cardFrame)
		{
			return new C3ir8(cardFrame);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardId"></param>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3ir8 InstantiateCard(uint cardId, Ci3MultiCardCage cardFrame)
		{
			return new C3ir8(cardId, cardFrame);
		}
	}
#else
    public sealed class C3Ir8Adapter : AbstractC3CardAdapter<C3Ir8AdapterSettings>
    {
    }
#endif
}
