using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class TaskComponent : AbstractVibeComponent
	{
		public event EventHandler OnTasksListUpdated;
		public event EventHandler OnForegroundTaskUpdated;

		private const string COMMAND = "tasks";
		private const string PARAM_TASKS_LIST = "list";
		private const string PARAM_TASKS_TOP = "top";
		private const string PARAM_TASKS_SWITCH = "switch {0}";

		private readonly List<TaskData> m_RunningTasks;
		private TaskData m_ForegroundTask;

		#region Properties

		[NotNull]
		public IEnumerable<TaskData> RunningTasks
		{
			get { return m_RunningTasks.AsReadOnly(); }
		}

		[CanBeNull]
		public TaskData ForegroundTask
		{
			get { return m_ForegroundTask; }
		}

		#endregion

		public TaskComponent(VibeBoard parent) : base(parent)
		{
			m_RunningTasks = new List<TaskData>();

			Subscribe(parent);
		}

		protected override void Dispose(bool disposing)
		{
			Unsubscribe(Parent);

			base.Dispose(disposing);
		}

		#region API Methods

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

		#endregion

		#region Parent Callbacks

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.RegisterResponseCallback<TaskListResponse>(TaskListCallback);
			vibe.ResponseHandler.RegisterResponseCallback<TaskTopResponse>(TaskTopCallback);
			vibe.ResponseHandler.RegisterResponseCallback<TaskSwitchResponse>(TaskSwitchCallback);
		}

		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.UnregisterResponseCallback<TaskListResponse>(TaskListCallback);
			vibe.ResponseHandler.UnregisterResponseCallback<TaskTopResponse>(TaskTopCallback);
			vibe.ResponseHandler.UnregisterResponseCallback<TaskSwitchResponse>(TaskSwitchCallback);
		}

		private void TaskListCallback(TaskListResponse response)
		{
			if (response.Error != null)
			{
				Log(eSeverity.Error, "Failed to list tasks - {0}", response.Error.Message);
				return;
			}

			m_RunningTasks.Clear();
			m_RunningTasks.AddRange(response.Value);
			
			Log(eSeverity.Informational, "Tasks list updated");
			OnTasksListUpdated.Raise(this);
		}

		private void TaskTopCallback(TaskTopResponse response)
		{
			if (response.Error != null)
			{
				Log(eSeverity.Error, "Failed to get foreground task - {0}", response.Error.Message);
			}

			m_ForegroundTask = response.Value;
			Log(eSeverity.Informational, "Task currently on top: {0}", response.Value.TopActivity);
			OnForegroundTaskUpdated.Raise(this);
		}

		private void TaskSwitchCallback(TaskSwitchResponse response)
		{
			if (response.Error != null)
			{
				Log(eSeverity.Error, "Failed to switch task - {0}", response.Error.Message);
				return;
			}

			Log(eSeverity.Informational, "Task switched successfully");
			TopTask();
		}

		#endregion

		#region Console

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (var command in base.GetConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("ListTasks", "Gets the list of running tasks", () => ListTasks());
			yield return new ConsoleCommand("TopTask", "Gets the foreground task", () => TopTask());
			yield return new GenericConsoleCommand<string>("SwitchTask", "Switches to the given task", (t) => SwitchTask(t));
		}

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Running Tasks", m_RunningTasks.Count);
			addRow("Top Task", m_ForegroundTask == null ? string.Empty : m_ForegroundTask.TopActivity);
		}

		#endregion
	}
}
