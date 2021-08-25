#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using System;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters
{
	public sealed class SuccessDataConverter : AbstractGenericJsonConverter<SuccessData>
	{
		private const string PROP_RESULT = "result";
		private const string VALUE_SUCCESS = "success";

		protected override void WriteProperties(JsonWriter writer, SuccessData value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.Success)
				writer.WriteProperty(PROP_RESULT, VALUE_SUCCESS);
		}

		protected override void ReadProperty(string property, JsonReader reader, SuccessData instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_RESULT:
					instance.Success = reader.GetValueAsString().Equals(VALUE_SUCCESS, StringComparison.OrdinalIgnoreCase);
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
