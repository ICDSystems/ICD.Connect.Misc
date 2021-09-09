using System;
using ICD.Connect.Misc.CrestronPro.InfinetEx;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.InetIoex
{
#if !NETSTANDARD
	public sealed class InetIoexIrcomAdapter : AbstractInfinetExAdapter<InetIoexIrcom, InetIoexIrcomAdatperSettings>, IPortParent
#else
	public sealed class InetIoexIrcomAdapter : AbstractInfinetExAdapter<InetIoexIrcomAdatperSettings>, IPortParent
#endif
	{

#if !NETSTANDARD
		protected override InetIoexIrcom InstantiateDevice(byte rfid, GatewayBase gateway)
		{
			return new InetIoexIrcom(rfid, gateway);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public ComPort GetComPort(int address)
		{
			if (Device == null)
				throw new InvalidOperationException("No device instantiated");

			if (address >= 1 && address <= Device.NumberOfComPorts)
				return Device.ComPorts[(uint)address];

			string message = string.Format("No {0} at address {1}", typeof(ComPort).Name, address);
			throw new InvalidOperationException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public IROutputPort GetIrOutputPort(int address)
		{
			if (Device == null)
				throw new InvalidOperationException("No device instantiated");

			if (address >= 1 && address <= Device.NumberOfIROutputPorts)
				return Device.IROutputPorts[(uint)address];

			string message = string.Format("No {0} at address {1}", typeof(IROutputPort).Name, address);
			throw new InvalidOperationException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public Relay GetRelayPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Relay).Name);
			throw new ArgumentOutOfRangeException("address", message);
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
	}
}