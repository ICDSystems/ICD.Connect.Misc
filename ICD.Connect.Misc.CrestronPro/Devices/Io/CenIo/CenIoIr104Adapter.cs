using System;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
#if SIMPLSHARP
	public sealed class CenIoIr104Adapter : AbstractCenIoAdapter<CenIoIr104, CenIoIr104AdapterSettings>
#else
	public sealed class CenIoIr104Adapter : AbstractCenIoAdapter<CenIoIr104AdapterSettings>
#endif
	{
#if SIMPLSHARP
		/// <summary>
		/// Creates a new instance of the wrapped internal device.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		protected override CenIoIr104 InstantiateDevice(CenIoIr104AdapterSettings settings)
		{
			return settings.Ipid == null
				   ? null
				   : new CenIoIr104(settings.Ipid.Value, ProgramInfo.ControlSystem);
		}
#endif
	}

#if SIMPLSHARP
	public abstract class AbstractCenIoIrAdapter<TDevice, TSettings> : AbstractCenIoAdapter<TDevice, TSettings>, ICenIoIrAdapter
		where TDevice : CenIoIr
#else
	public abstract class AbstractCenIoIrAdapter<TSettings> : AbstractCenIoAdapter<TSettings>, ICenIoIrAdapter
#endif
		where TSettings : ICenIoIrAdapterSettings, new()
	{
#if SIMPLSHARP
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public override IROutputPort GetIrOutputPort(int address)
		{
			if (Device == null)
				throw new InvalidOperationException("No device instantiated");

			if (address >= 1 && address <= Device.NumberOfIROutputPorts)
				return Device.IROutputPorts[(uint)address];

			string message = string.Format("No {0} at address {1}", typeof(IROutputPort).Name, address);
			throw new InvalidOperationException(message);
		}
#endif
	}

	public interface ICenIoIrAdapter : ICenIoAdapter
	{
	}
}
