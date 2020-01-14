using System;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using ICD.Connect.Misc.Unsplash.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash.Converters
{
	public sealed class UnsplashCollectionResultConverter : AbstractGenericJsonConverter<UnsplashCollectionResult>
	{
		private const string ATTRIBUTE_ID = "id";
		private const string ATTRIBUTE_TITLE = "title";
		private const string ATTRIBUTE_DESCRIPTION = "description";
		private const string ATTRIBUTE_CURATED = "curated";
		private const string ATTRIBUTE_FEATURED = "featured";
		private const string ATTRIBUTE_TOTAL_PHOTOS = "total_photos";
		private const string ATTRIBUTE_PRIVATE = "private";
		private const string ATTRIBUTE_SHARED_KEY = "share_key";
		private const string ATTRIBUTE_LINKS = "links";
		private const string ATTRIBUTE_USER = "user";

		protected override void ReadProperty(string property, JsonReader reader, UnsplashCollectionResult instance,
		                                     JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_ID:
					instance.Id = Convert.ToDouble(reader.GetValueAsString());
					break;
				case ATTRIBUTE_TITLE:
					instance.Title = reader.GetValueAsString();
					break;
				case ATTRIBUTE_DESCRIPTION:
					instance.Description = reader.GetValueAsString();
					break;
				case ATTRIBUTE_CURATED:
					instance.Curated = reader.GetValueAsBool();
					break;
				case ATTRIBUTE_FEATURED:
					instance.Features = reader.GetValueAsBool();
					break;
				case ATTRIBUTE_TOTAL_PHOTOS:
					instance.TotalPhotos = Convert.ToDouble(reader.GetValueAsString());
					break;
				case ATTRIBUTE_PRIVATE:
					instance.Private = reader.GetValueAsBool();
					break;
				case ATTRIBUTE_SHARED_KEY:
					instance.ShareKey = reader.GetValueAsString();
					break;
				case ATTRIBUTE_LINKS:
					instance.Links = serializer.Deserialize<UnsplashCollectionLinks>(reader);
					break;
				case ATTRIBUTE_USER:
					instance.User = serializer.Deserialize<UnsplashCollectionUser>(reader);
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}

	}
}
