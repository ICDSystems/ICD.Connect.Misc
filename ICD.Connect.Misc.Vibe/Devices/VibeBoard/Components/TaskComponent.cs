namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class TaskComponent : AbstractVibeComponent
	{
		private const string COMMAND = "tasks";
		private const string PARAM_TASKS_LIST = "list";
		private const string PARAM_TASKS_TOP = "top";
		private const string PARAM_TASKS_SWITCH = "switch {0}";

		public TaskComponent(VibeBoard parent) : base(parent)
		{
		}

		public void ListTasks()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_TASKS_LIST));
		}

		public void TopTask()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_TASKS_TOP));
		}

		public void SwitchTask(string task)
		{
			string param = string.Format(PARAM_TASKS_SWITCH, task);
			Parent.SendCommand(new VibeCommand(COMMAND, param));
		}
	}
}
