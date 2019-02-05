using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
	[KrangSettings("CenIoCom102", typeof(CenIoCom102Adapter))]
	public sealed class CenIoCom102AdapterSettings : AbstractCenIoComAdapterSettings
	{
	}

	public abstract class AbstractCenIoComAdapterSettings : AbstractCenIoAdapterSettings, ICenIoComAdapterSettings
	{
	}

	public interface ICenIoComAdapterSettings : ICenIoAdapterSettings
	{
	}
}