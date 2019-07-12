using System;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
#endif
using ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBase;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBaseWithVersiport
{
#if SIMPLSHARP
	public abstract class AbstractC2nCbdPBaseWithVersiportAdapter<TKeypad, TSettings> : AbstractC2nCbdPBaseAdapter<TKeypad, TSettings>, IC2nCbdPBaseWithVersiportAdapter
		where TKeypad : Crestron.SimplSharpPro.Keypads.C2nCbdPBaseWithVersiport
#else
	public abstract class AbstractC2nCbdPBaseWithVersiportAdapter<TSettings> : AbstractC2nCbdPBaseAdapter<TSettings>, IC2nCbdPBaseWithVersiportAdapter
#endif
		where TSettings: IC2nCbdPBaseWithVersiportAdapterSettings, new()
	{
		#region IO

#if SIMPLSHARP
		/// <summary>
		/// Gets the port at the given addres.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual ComPort GetComPort(int address)
		{
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(ComPort).Name, address);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given addres.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual IROutputPort GetIrOutputPort(int address)
		{
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(IROutputPort).Name, address);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual Relay GetRelayPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Relay).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual Versiport GetIoPort(int address)
		{
			if (Keypad.VersiPorts == null)
				throw new NotSupportedException("Keypad has no VersiPorts");

			if (address < 0 || !Keypad.VersiPorts.Contains((uint)address))
				throw new ArgumentOutOfRangeException("address", string.Format("{0} has no VersiPort at address {1}", this, address));

			return Keypad.VersiPorts[(uint)address];
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public DigitalInput GetDigitalInputPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(DigitalInput).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="io"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public Cec GetCecPort(eInputOuptut io, int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Cec).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}
#endif

		#endregion
	}
}