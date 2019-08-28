namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class StartComponent : AbstractVibeComponent
	{
		private const string COMMAND = "start";
		private const string PARAMS = "-n {0}/{1}";

		public StartComponent(VibeBoard parent) : base(parent)
		{
		}
		
		public void StartActivity(string packageName, string activityName)
		{
			string param = string.Format(PARAMS, packageName, activityName);
			Parent.SendCommand(new VibeCommand(COMMAND, param));
		}
	}
}
