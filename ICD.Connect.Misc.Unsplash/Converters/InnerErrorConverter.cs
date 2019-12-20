using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using ICD.Connect.Misc.Unsplash_NetStandard.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash_NetStandard.Converters
{
	public sealed class InnerErrorConverter : AbstractGenericJsonConverter<InnerError>
	{
		private const string ATTRIBUTE_REQUEST_ID = "requester-id";
		private const string ATTRIBUTE_DATE = "date";

		protected override void ReadProperty(string property, JsonReader reader, InnerError instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_REQUEST_ID:
					instance.ResquestId = reader.GetValueAsString();
					break;
				case ATTRIBUTE_DATE:
					instance.Date = reader.GetValueAsString();
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
