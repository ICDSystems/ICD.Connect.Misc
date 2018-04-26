using ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBaseWithVersiport;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdP
{
	[KrangSettings("C2nCbdP", typeof(C2nCbdPAdapter))]
	public sealed class C2nCbdPAdapterSettings : AbstractC2nCbdPBaseWithVersiportAdapterSettings, IC2nCbdPAdapterSettings
	{
	}
}
