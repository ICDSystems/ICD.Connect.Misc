using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Io.CenIo
{
	[KrangSettings("CenIoDi104", typeof(CenIoDi104Adapter))]
	public sealed class CenIoDi104AdapterSettings : AbstractCenIoDiAdapterSettings
	{
	}

	public abstract class AbstractCenIoDiAdapterSettings : AbstractCenIoAdapterSettings, ICenIoDiAdapterSettings
	{
	}

	public interface ICenIoDiAdapterSettings : ICenIoAdapterSettings
	{
	}
}