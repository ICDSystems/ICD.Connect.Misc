using System;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using ICD.Connect.Misc.Unsplash_NetStandard.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash_NetStandard.Converters
{
	public sealed class UnsplashPhotoUrlConverter : AbstractGenericJsonConverter<UnsplashPhotoUrls>
	{
		private const string ATTRIBUTE_RAW = "raw";
		private const string ATTRIBUTE_FULL = "full";
		private const string ATTRIBUTE_REGULAR = "regular";
		private const string ATTRIBUTE_SMALL = "small";
		private const string ATTRIBUTE_THUMB = "thumb";

		protected override void ReadProperty(string property, JsonReader reader, UnsplashPhotoUrls instance,
		                                     JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_RAW:
					instance.Raw = reader.GetValueAsString();
					break;
				case ATTRIBUTE_FULL:
					instance.Full = reader.GetValueAsString();
					break;
				case ATTRIBUTE_REGULAR:
					instance.Regular = reader.GetValueAsString();
					break;
				case ATTRIBUTE_SMALL:
					instance.Small = reader.GetValueAsString();
					break;
				case ATTRIBUTE_THUMB:
					instance.Thumb = reader.GetValueAsString();
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
