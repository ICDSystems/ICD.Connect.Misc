namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class DumpComponent : AbstractVibeComponent
	{
		private const string COMMAND = "dump";
		private const string PARAM_USB = "usb";

		public DumpComponent(VibeBoard parent)
			: base(parent)
		{
		}

		public void DumpUsbDevices()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_USB));
		}
	}
}
