namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class PackageComponent : AbstractVibeComponent
	{
		private const string COMMAND = "packages";
		private const string PARAM_LIST = "list";

		public PackageComponent(VibeBoard parent) : base(parent)
		{
		}

		public void ListPackages()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_LIST));
		}
	}
}
