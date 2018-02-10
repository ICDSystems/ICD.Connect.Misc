namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbdBase
{
	public interface IC2nCbdBaseAdapterSettings : IInetCbdexAdapterSettings
	{
		 byte? CresnetId { get; set; }
	}
}