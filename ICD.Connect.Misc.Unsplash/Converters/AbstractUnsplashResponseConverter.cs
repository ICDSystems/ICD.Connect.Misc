using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using ICD.Connect.Misc.Unsplash.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash.Converters
{
	public abstract class AbstractUnsplashResponseConverter<T> : AbstractGenericJsonConverter<T>
		where T : AbstractUnsplashResponse
	{
		private const string ATTRIBUTE_ERROR = "error";
		private const string ATTRIBUTE_ERROR_DESCRIPTION = "error_description";

		protected override void ReadProperty(string property, JsonReader reader, T instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_ERROR:
					instance.Error = reader.GetValueAsString();
					break;
				case ATTRIBUTE_ERROR_DESCRIPTION:
					instance.ErrorDescription = reader.GetValueAsString();
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
