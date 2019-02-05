using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
	[KrangSettings("CenIoRy104", typeof(CenIoRy104Adapter))]
	public sealed class CenIoRy104AdapterSettings : AbstractCenIoRyAdapterSettings
	{
	}

	public abstract class AbstractCenIoRyAdapterSettings : AbstractCenIoAdapterSettings, ICenIoRyAdapterSettings
	{
	}

	public interface ICenIoRyAdapterSettings : ICenIoAdapterSettings
	{
	}
}