using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters
{
	public class AbstractVibeResponseConverter<TResponse, TData> : AbstractGenericJsonConverter<TResponse>
		where TResponse : AbstractVibeResponse<TData>
	{
		private const string PROP_TYPE = "type";
		private const string PROP_RESULT_ID = "resultId";
		private const string PROP_SYNC = "sync";
		private const string PROP_VALUE = "value";
		private const string PROP_ERROR_ID = "id";
		private const string PROP_ERROR_DATA = "error";

		protected override void WriteProperties(JsonWriter writer, TResponse instance, JsonSerializer serializer)
		{
			base.WriteProperties(writer, instance, serializer);

			writer.WriteProperty(PROP_TYPE, instance.Type);
			writer.WriteProperty(PROP_RESULT_ID, instance.ResultId);
			writer.WriteProperty(PROP_SYNC, instance.Sync);

			if (instance.Value != null)
			{
				writer.WritePropertyName(PROP_VALUE);
				serializer.Serialize(writer, instance.Value);
			}

			if (instance.ErrorId != null)
				writer.WriteProperty(PROP_ERROR_ID, instance.ErrorId);

			if (instance.Error != null)
			{
				writer.WritePropertyName(PROP_ERROR_DATA);
				serializer.Serialize(writer, instance.Error);
			}
		}

		protected override void ReadProperty(string property, JsonReader reader, TResponse instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_TYPE:
					instance.Type = reader.GetValueAsString();
					break;

				case PROP_RESULT_ID:
					instance.ResultId = reader.GetValueAsString();
					break;

				case PROP_SYNC:
					instance.Sync = reader.GetValueAsBool();
					break;

				case PROP_VALUE:
					instance.Value = serializer.Deserialize<TData>(reader);
					break;

				case PROP_ERROR_ID:
					instance.ErrorId = reader.GetValueAsString();
					break;

				case PROP_ERROR_DATA:
					instance.Error = serializer.Deserialize<ErrorData>(reader);
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}

	public sealed class ErrorDataConverter : AbstractGenericJsonConverter<ErrorData>
	{
		private const string PROP_CODE = "code";
		private const string PROP_MESSAGE = "message";

		protected override void WriteProperties(JsonWriter writer, ErrorData value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.Code != null)
				writer.WriteProperty(PROP_CODE, value.Code);

			if (value.Message != null)
				writer.WriteProperty(PROP_MESSAGE, value.Message);
		}

		protected override void ReadProperty(string property, JsonReader reader, ErrorData instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_CODE:
					instance.Code = reader.GetValueAsString();
					break;

				case PROP_MESSAGE:
					instance.Message = reader.GetValueAsString();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
