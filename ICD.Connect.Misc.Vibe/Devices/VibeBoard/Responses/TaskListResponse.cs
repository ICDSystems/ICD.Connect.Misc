using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;
using Newtonsoft.Json;
using System;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(TaskListResponseConverter))]
	public sealed class TaskListResponse : AbstractVibeResponse<TaskData[]>
	{
	}

	[JsonConverter(typeof(TaskDataConverter))]
	public sealed class TaskData
	{
		public int TaskId { get; set; }

		public int StackId { get; set; }

		public DateTime LastActiveTime { get; set; }

		public string TopActivity { get; set; }
	}
}
