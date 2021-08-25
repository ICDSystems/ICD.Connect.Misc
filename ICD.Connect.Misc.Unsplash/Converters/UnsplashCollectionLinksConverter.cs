#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using ICD.Connect.Misc.Unsplash.Responses;

namespace ICD.Connect.Misc.Unsplash.Converters
{
	public sealed class UnsplashCollectionLinksConverter : AbstractGenericJsonConverter<UnsplashCollectionLinks>
	{
		private const string ATTRIBUTE_SELF = "self";
		private const string ATTRIBUTE_HTML = "html";
		private const string ATTRIBUTE_PHOTOS = "photos";
		private const string ATTRIBUTE_RELATED = "related";

		protected override void ReadProperty(string property, JsonReader reader, UnsplashCollectionLinks instance,
		                                     JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_SELF:
					instance.Self = reader.GetValueAsString();
					break;
				case ATTRIBUTE_HTML:
					instance.Html = reader.GetValueAsString();
					break;
				case ATTRIBUTE_PHOTOS:
					instance.Photos = reader.GetValueAsString();
					break;
				case ATTRIBUTE_RELATED:
					instance.Related = reader.GetValueAsString();
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
