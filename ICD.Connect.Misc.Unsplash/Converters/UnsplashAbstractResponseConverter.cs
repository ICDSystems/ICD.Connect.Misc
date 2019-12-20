using ICD.Common.Utils.Json;
using ICD.Connect.Misc.Unsplash_NetStandard.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash_NetStandard.Converters
{
	public abstract class UnsplashAbstractResponseConverter<T> : AbstractGenericJsonConverter<T>
		where T : UnsplashAbstractResponse
	{
		private const string ATTRIBUTE_ERROR = "error";

		protected override void ReadProperty(string property, JsonReader reader, T instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_ERROR:
					instance.Error = serializer.Deserialize<Error>(reader);
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
