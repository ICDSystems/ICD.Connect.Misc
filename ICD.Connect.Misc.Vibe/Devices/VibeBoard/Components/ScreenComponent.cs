namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class ScreenComponent : AbstractVibeComponent
	{
		private const string COMMAND = "screen";
		private const string PARAM_SCREEN_ON = "on";
		private const string PARAM_SCREEN_OFF = "off";

		public ScreenComponent(VibeBoard parent) : base(parent)
		{
		}

		public void SetScreenState(bool on)
		{
			Parent.SendCommand(new VibeCommand(COMMAND, on ? PARAM_SCREEN_ON : PARAM_SCREEN_OFF));
		}

		public void ScreenOn()
		{
			SetScreenState(true);
		}

		public void ScreenOff()
		{
			SetScreenState(false);
		}
	}
}
