#if SIMPLSHARP
using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.ThreeSeriesCards;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
#if SIMPLSHARP
    public abstract class AbstractC3RyAdapter<TCard, TSettings> : AbstractCardAdapter<TCard, TSettings>
		where TCard : C3ry
		where TSettings : AbstractC3RyAdapterSettings, new()
	{
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public override Relay GetRelayPort(int address)
		{
			if (Card.RelayPorts.Contains((uint)address))
				return Card.RelayPorts[(uint)address];

			string message = string.Format("{0} has no {1} with address {2}", this, typeof(Relay).Name, address);
			throw new KeyNotFoundException(message);
		}
	}
#else
    public abstract class AbstractC3RyAdapter<TSettings> : AbstractCardAdapter<TSettings>
        where TSettings : AbstractC3RyAdapterSettings, new()
    {
    }
#endif
}
