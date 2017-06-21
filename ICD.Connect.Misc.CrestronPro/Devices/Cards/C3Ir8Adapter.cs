using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.ThreeSeriesCards;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public sealed class C3Ir8Adapter : AbstractCardAdapter<C3ir8, C3Ir8AdapterSettings>
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
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3ir8 InstantiateCard(CenCi31 cardFrame)
		{
			return new C3ir8(cardFrame);
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected override C3ir8 InstantiateCard(byte ipid, CenCi33 cardFrame)
		{
			return new C3ir8(ipid, cardFrame);
		}
	}
}
