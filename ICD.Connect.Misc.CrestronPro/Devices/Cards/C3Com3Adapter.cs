using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.ThreeSeriesCards;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public sealed class C3Com3Adapter : AbstractCardAdapter<C3com3, C3Com3AdapterSettings>
	{
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public override ComPort GetComPort(int address)
		{
			if (Card.ComPorts.Contains((uint)address))
				return Card.ComPorts[(uint)address];

			string message = string.Format("{0} has no {1} with address {2}", this, typeof(ComPort).Name, address);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3com3 InstantiateCard(CenCi31 cardFrame)
		{
			return new C3com3(cardFrame);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3com3 InstantiateCard(byte ipid, CenCi33 cardFrame)
		{
			return new C3com3(ipid, cardFrame);
		}
	}
}
