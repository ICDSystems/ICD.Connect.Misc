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
	public sealed class UnsplashPhotoLinkConverter : AbstractGenericJsonConverter<UnsplashPhotoLinks>
	{
		private const string ATTRIBUTE_SELF = "self";
		private const string ATTRIBUTE_HTML = "html";
		private const string ATTRIBUTE_DOWNLOAD = "download";
		private const string ATTRIBUTE_DOWNLOAD_LOCATION = "download_location";

		protected override void ReadProperty(string property, JsonReader reader, UnsplashPhotoLinks instance,
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
				case ATTRIBUTE_DOWNLOAD:
					instance.Download = reader.GetValueAsString();
					break;
				case ATTRIBUTE_DOWNLOAD_LOCATION:
					instance.DownloadLocation = reader.GetValueAsString();
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
