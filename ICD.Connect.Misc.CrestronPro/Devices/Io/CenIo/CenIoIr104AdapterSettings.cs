using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
	[KrangSettings("CenIoIr104", typeof(CenIoIr104Adapter))]
	public sealed class CenIoIr104AdapterSettings : AbstractCenIoIrAdapterSettings
	{
	}

	public abstract class AbstractCenIoIrAdapterSettings : AbstractCenIoAdapterSettings, ICenIoIrAdapterSettings
	{
	}

	public interface ICenIoIrAdapterSettings : ICenIoAdapterSettings
	{
	}
}