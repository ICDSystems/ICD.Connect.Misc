using System;
using ICD.Connect.Misc.CrestronPro.InfinetEx;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.InetIoex
{
#if SIMPLSHARP
	public sealed class InetIoexRyioAdapter : AbstractInfinetExAdapter<InetIoexRyio, InetIoexRyioAdapterSettings>, IPortParent
#else
	public sealed class InetIoexRyioAdapter : AbstractInfinetExAdapter<InetIoexRyioAdapterSettings>
#endif
	{

#if SIMPLSHARP

		protected override InetIoexRyio InstantiateDevice(byte rfid, GatewayBase gateway)
		{
			return new InetIoexRyio(rfid, gateway);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public ComPort GetComPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(ComPort).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public IROutputPort GetIrOutputPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(IROutputPort).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public Relay GetRelayPort(int address)
		{
			if (Device == null)
				throw new InvalidOperationException("No device instantiated");

			if (address >= 1 && address <= Device.NumberOfRelayPorts)
				return Device.RelayPorts[(uint)address];

			string message = string.Format("No {0} at address {1}", typeof(Relay).Name, address);
			throw new InvalidOperationException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public Versiport GetIoPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Versiport).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public DigitalInput GetDigitalInputPort(int address)
		{
			if (Device == null)
				throw new InvalidOperationException("No device instantiated");

			if (address >= 1 && address <= Device.NumberOfDigitalInputPorts)
				return Device.DigitalInputPorts[(uint)address];

			string message = string.Format("No {0} at address {1}", typeof(DigitalInput).Name, address);
			throw new InvalidOperationException(message);
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
	}
}