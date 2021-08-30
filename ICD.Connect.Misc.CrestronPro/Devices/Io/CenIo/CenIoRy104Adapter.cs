using System;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
#if !NETSTANDARD
	public sealed class CenIoRy104Adapter : AbstractCenIoRyAdapter<CenIoRy104, CenIoRy104AdapterSettings>
#else
	public sealed class CenIoRy104Adapter : AbstractCenIoRyAdapter<CenIoRy104AdapterSettings>
#endif
	{
#if !NETSTANDARD
		/// <summary>
		/// Creates a new instance of the wrapped internal device.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		protected override CenIoRy104 InstantiateDevice(CenIoRy104AdapterSettings settings)
		{
			return settings.Ipid == null
				   ? null
				   : new CenIoRy104(settings.Ipid.Value, ProgramInfo.ControlSystem);
		}
#endif
	}

#if !NETSTANDARD
	public abstract class AbstractCenIoRyAdapter<TDevice, TSettings> : AbstractCenIoAdapter<TDevice, TSettings>, ICenIoRyAdapter
		where TDevice : CenIoRy
#else
	public abstract class AbstractCenIoRyAdapter<TSettings> : AbstractCenIoAdapter<TSettings>, ICenIoRyAdapter
#endif
		where TSettings : ICenIoRyAdapterSettings, new()
	{
#if !NETSTANDARD
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public override Relay GetRelayPort(int address)
		{
			if (Device == null)
				throw new InvalidOperationException("No device instantiated");

			if (address >= 1 && address <= Device.NumberOfRelayPorts)
				return Device.RelayPorts[(uint)address];

			string message = string.Format("No {0} at address {1}", typeof(Relay).Name, address);
			throw new InvalidOperationException(message);
		}
#endif
	}

	public interface ICenIoRyAdapter : ICenIoAdapter
	{
	}
}
