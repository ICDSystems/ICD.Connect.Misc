using System;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
#if SIMPLSHARP
	public sealed class CenIoDi104Adapter : AbstractCenIoAdapter<CenIoDi104, CenIoDi104AdapterSettings>
#else
	public sealed class CenIoDi104Adapter : AbstractCenIoAdapter<CenIoDi104AdapterSettings>
#endif
	{
#if SIMPLSHARP
		/// <summary>
		/// Creates a new instance of the wrapped internal device.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		protected override CenIoDi104 InstantiateDevice(CenIoDi104AdapterSettings settings)
		{
			return settings.Ipid == null
				   ? null
				   : new CenIoDi104(settings.Ipid.Value, ProgramInfo.ControlSystem);
		}
#endif
	}

#if SIMPLSHARP
	public abstract class AbstractCenIoDiAdapter<TDevice, TSettings> : AbstractCenIoAdapter<TDevice, TSettings>, ICenIoDiAdapter
		where TDevice : CenIoDi
#else
	public abstract class AbstractCenIoDiAdapter<TSettings> : AbstractCenIoAdapter<TSettings>, ICenIoDiAdapter
#endif
		where TSettings : ICenIoDiAdapterSettings, new()
	{
#if SIMPLSHARP
		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public override DigitalInput GetDigitalInputPort(int address)
		{
			if (Device == null)
				throw new InvalidOperationException("No device instantiated");

			if (address >= 1 && address <= Device.NumberOfDigitalInputPorts)
				return Device.DigitalInputPorts[(uint)address];

			string message = string.Format("No {0} at address {1}", typeof(DigitalInput).Name, address);
			throw new InvalidOperationException(message);
		}
#endif
	}

	public interface ICenIoDiAdapter : ICenIoAdapter
	{
	}
}
