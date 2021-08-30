using System;
#if !NETSTANDARD
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
#if !NETSTANDARD
	public sealed class CenIoCom102Adapter : AbstractCenIoComAdapter<CenIoCom102, CenIoCom102AdapterSettings>
#else
	public sealed class CenIoCom102Adapter : AbstractCenIoComAdapter<CenIoCom102AdapterSettings>
#endif
	{
#if !NETSTANDARD
		/// <summary>
		/// Creates a new instance of the wrapped internal device.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		protected override CenIoCom102 InstantiateDevice(CenIoCom102AdapterSettings settings)
		{
			return settings.Ipid == null
				   ? null
				   : new CenIoCom102(settings.Ipid.Value, ProgramInfo.ControlSystem);
		}
#endif
	}

#if !NETSTANDARD
	public abstract class AbstractCenIoComAdapter<TDevice, TSettings> : AbstractCenIoAdapter<TDevice, TSettings>, ICenIoComAdapter
		where TDevice : CenIoCom
#else
	public abstract class AbstractCenIoComAdapter<TSettings> : AbstractCenIoAdapter<TSettings>, ICenIoComAdapter
#endif
		where TSettings : ICenIoComAdapterSettings, new()
	{
#if !NETSTANDARD
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public override ComPort GetComPort(int address)
		{
			if (Device == null)
				throw new InvalidOperationException("No device instantiated");

			if (address >= 1 && address <= Device.NumberOfComPorts)
				return Device.ComPorts[(uint)address];

			string message = string.Format("No {0} at address {1}", typeof(ComPort).Name, address);
			throw new InvalidOperationException(message);
		}
#endif
	}

	public interface ICenIoComAdapter : ICenIoAdapter
	{
	}
}
