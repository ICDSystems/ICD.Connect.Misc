using System;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using ICD.Connect.Misc.Unsplash_NetStandard.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash_NetStandard.Converters
{
	public sealed class UnsplashPhotoLinkConverter : AbstractGenericJsonConverter<UnsplashPhotoLinkResponse>
	{
		private const string ATTRIBUTE_SELF = "self";
		private const string ATTRIBUTE_HTML = "html";
		private const string ATTRIBUTE_DOWNLOAD = "download";
		private const string ATTRIBUTE_DOWNLOAD_LOCATION = "download_location";

		protected override void ReadProperty(string property, JsonReader reader, UnsplashPhotoLinkResponse instance,
		                                     JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_SELF:
					instance.Self = new Uri(reader.GetValueAsString());
					break;
				case ATTRIBUTE_HTML:
					instance.Html = new Uri(reader.GetValueAsString());
					break;
				case ATTRIBUTE_DOWNLOAD:
					instance.Download = new Uri(reader.GetValueAsString());
					break;
				case ATTRIBUTE_DOWNLOAD_LOCATION:
					instance.DownloadLocation = new Uri(reader.GetValueAsString());
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
