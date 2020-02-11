using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;
using System;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters
{
	public sealed class TaskDataConverter : AbstractGenericJsonConverter<TaskData>
	{
		private const string PROP_TASK_ID = "taskId";
		private const string PROP_STACK_ID = "stackId";
		private const string PROP_LAST_ACTIVE_TIME = "lastActiveTime";
		private const string PROP_TOP_ACTIVITY = "topActivity";

		protected override void WriteProperties(JsonWriter writer, TaskData value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.TaskId != default(int))
				writer.WriteProperty(PROP_TASK_ID, value.TaskId);

			if (value.StackId != default(int))
				writer.WriteProperty(PROP_STACK_ID, value.StackId);

			if (value.LastActiveTime != default(DateTime))
				writer.WriteProperty(PROP_LAST_ACTIVE_TIME, value.LastActiveTime);

			if (value.TopActivity != null)
				writer.WriteProperty(PROP_TOP_ACTIVITY, value.TopActivity);
		}

		protected override void ReadProperty(string property, JsonReader reader, TaskData instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_TASK_ID:
					instance.TaskId = reader.GetValueAsInt();
					break;

				case PROP_STACK_ID:
					instance.StackId = reader.GetValueAsInt();
					break;

				case PROP_LAST_ACTIVE_TIME:
					instance.LastActiveTime = DateTimeUtils.FromEpochMilliseconds(reader.GetValueAsLong());
					break;

				case PROP_TOP_ACTIVITY:
					instance.TopActivity = reader.GetValueAsString();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
