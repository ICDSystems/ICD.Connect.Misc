using System;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using ICD.Connect.Misc.Unsplash_NetStandard.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash_NetStandard.Converters
{
	public sealed class UnsplashPhotoResponseConverter : AbstractGenericJsonConverter<UnsplashPhotoResponse>
	{
		private const string ATTRIBUTE_ID = "id";
		private const string ATTRIBUTE_CREATED_AT = "created_at";
		private const string ATTRIBUTE_UPDATED_AT = "updated_at";
		private const string ATTRIBUTE_PROMOTED_AT = "promoted_at";
		private const string ATTRIBUTE_WIDTH = "width";
		private const string ATTRIBUTE_HEIGHT = "height";
		private const string ATTRIBUTE_COLOR = "color";
		private const string ATTRIBUTE_DESCRIPTION = "descriptions";
		private const string ATTRIBUTE_ALT_DESCRIPTION = "alt_description";
		private const string ATTRIBUTE_URLS = "urls";
		private const string ATTRIBUTE_LINKS = "links";
		private const string ATTRIBUTE_CATEGORIES = "categories";
		private const string ATTRIBUTE_LIKES = "likes";
		private const string ATTRIBUTE_LIKED_BY_USER = "lides_by_user";
		private const string ATTRIBUTE_USER = "user";

		protected override void ReadProperty(string property, JsonReader reader, UnsplashPhotoResponse instance,
		                                     JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_ID:
					instance.Id = reader.GetValueAsString();
					break;
				case ATTRIBUTE_CREATED_AT:
					instance.CreatedAt = reader.GetValueAsDateTime();
					break;
				case ATTRIBUTE_UPDATED_AT:
					instance.UpdatedAt = reader.GetValueAsDateTime();
					break;
				case ATTRIBUTE_PROMOTED_AT:
					instance.PromotedAt = reader.GetValueAsDateTime();
					break;
				case ATTRIBUTE_WIDTH:
					instance.Width = Convert.ToDouble(reader.GetValueAsString());
					break;
				case ATTRIBUTE_HEIGHT:
					instance.Height = Convert.ToDouble(reader.GetValueAsString());
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
					instance.Urls = serializer.Deserialize<UnsplashPhotoUrlResponse>(reader);
					break;
				case ATTRIBUTE_LINKS:
					instance.Links = serializer.Deserialize<UnsplashPhotoLinkResponse>(reader);
					break;
				case ATTRIBUTE_CATEGORIES:
					instance.Categories = serializer.DeserializeArray<UnsplashPhotoCategoryResponse>(reader).ToArray();
					break;
				case ATTRIBUTE_LIKES:
					instance.Likes = Convert.ToDouble(reader.GetValueAsString());
					break;
				case ATTRIBUTE_LIKED_BY_USER:
					instance.LikesByUser = reader.GetValueAsBool();
					break;
				case ATTRIBUTE_USER:
					instance.User = serializer.Deserialize<UnsplashPhotoUserResponse>(reader);
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
