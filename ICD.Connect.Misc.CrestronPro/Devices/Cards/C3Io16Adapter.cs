﻿using System;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.ThreeSeriesCards;
using Crestron.SimplSharpProInternal;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
#if !NETSTANDARD
	public sealed class C3Io16Adapter : AbstractC3CardAdapter<C3io16, C3Io16AdapterSettings>
	{
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public override Versiport GetIoPort(int address)
		{
			if (Card.VersiPorts.Contains((uint)address))
				return Card.VersiPorts[(uint)address];

			string message = string.Format("{0} has no {1} with address {2}", this, typeof(Versiport).Name, address);
			throw new IndexOutOfRangeException(message);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3io16 InstantiateCard(Ci3SingleCardCage cardFrame)
		{
			return new C3io16(cardFrame);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardId"></param>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3io16 InstantiateCard(uint cardId, Ci3MultiCardCage cardFrame)
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
