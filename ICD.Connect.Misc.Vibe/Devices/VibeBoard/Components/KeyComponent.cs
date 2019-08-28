namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class KeyComponent : AbstractVibeComponent
	{
		private const string COMMAND = "key";

		public KeyComponent(VibeBoard parent) : base(parent)
		{
		}

		public void KeyPress(eVibeKey key)
		{
			Parent.SendCommand(new VibeCommand(COMMAND, key.ToString().ToLower()));
		}
	}

	public enum eVibeKey
	{
		Back,
		Home,
		Up,
		Down,
		Left,
		Right,
		Task
	}
}
