using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using ICD.Connect.Misc.Unsplash_NetStandard.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash_NetStandard.Converters
{
	public sealed class UnsplashPhotoUserConverter : AbstractGenericJsonConverter<UnsplashPhotoUserResponse>
	{
		private const string ATTRIBUTE_USER_ID = "id";
		private const string ATTRIBUTE_USER_NAME = "username";
		private const string ATTRIBUTE_NAME = "name";

		protected override void ReadProperty(string property, JsonReader reader, UnsplashPhotoUserResponse instance,
		                                     JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_USER_ID:
					instance.UserId = reader.GetValueAsString();
					break;
				case ATTRIBUTE_USER_NAME:
					instance.UserName = reader.GetValueAsString();
					break;
				case ATTRIBUTE_NAME:
					instance.Name = reader.GetValueAsString();
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
