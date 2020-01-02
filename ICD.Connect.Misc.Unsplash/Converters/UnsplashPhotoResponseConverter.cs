using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using ICD.Connect.Misc.Unsplash.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash.Converters
{
	public sealed class UnsplashPhotoResponseConverter : AbstractGenericJsonConverter<UnsplashPhotoResult>
	{
		private const string ATTRIBUTE_ID = "id";
		private const string ATTRIBUTE_WIDTH = "width";
		private const string ATTRIBUTE_HEIGHT = "height";
		private const string ATTRIBUTE_COLOR = "color";
		private const string ATTRIBUTE_DESCRIPTION = "description";
		private const string ATTRIBUTE_ALT_DESCRIPTION = "alt_description";
		private const string ATTRIBUTE_URLS = "urls";
		private const string ATTRIBUTE_LINKS = "links";
		private const string ATTRIBUTE_CATEGORIES = "categories";
		private const string ATTRIBUTE_USER = "user";

		protected override void ReadProperty(string property, JsonReader reader, UnsplashPhotoResult instance,
		                                     JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_ID:
					instance.Id = reader.GetValueAsString();
					break;
				case ATTRIBUTE_WIDTH:
					instance.Width = reader.GetValueAsString();
					break;
				case ATTRIBUTE_HEIGHT:
					instance.Height = reader.GetValueAsString();
					break;
				case ATTRIBUTE_COLOR:
					instance.Color = reader.GetValueAsString();
					break;
				case ATTRIBUTE_DESCRIPTION:
					instance.Description = reader.GetValueAsString();
					break;
				case ATTRIBUTE_ALT_DESCRIPTION:
					instance.AltDescrption = reader.GetValueAsString();
					break;
				case ATTRIBUTE_URLS:
					instance.Urls = serializer.Deserialize<UnsplashPhotoUrls>(reader);
					break;
				case ATTRIBUTE_LINKS:
					instance.Links = serializer.Deserialize<UnsplashPhotoLinks>(reader);
					break;
				case ATTRIBUTE_CATEGORIES:
					instance.Categories = serializer.DeserializeArray<UnsplashPhotoCategory>(reader).ToArray();
					break;
				case ATTRIBUTE_USER:
					instance.User = serializer.Deserialize<UnsplashPhotoUser>(reader);
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
