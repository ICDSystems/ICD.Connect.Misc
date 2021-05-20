using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;
using System;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters
{
	public sealed class ScreenDataConverter : AbstractGenericJsonConverter<ScreenData>
	{
		private const string PROP_RESULT = "result";
		private const string VALUE_SUCCESS = "success";
		private const string PROP_STATE = "state";
		private const string VALUE_ON = "on";

		protected override void WriteProperties(JsonWriter writer, ScreenData value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.Success)
				writer.WriteProperty(PROP_RESULT, VALUE_SUCCESS);
		}

		protected override void ReadProperty(string property, JsonReader reader, ScreenData instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_RESULT:
					instance.Success = reader.GetValueAsString().Equals(VALUE_SUCCESS, StringComparison.OrdinalIgnoreCase);
					break;

				case PROP_STATE:
					instance.State = reader.GetValueAsString().Equals(VALUE_ON, StringComparison.OrdinalIgnoreCase);
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
