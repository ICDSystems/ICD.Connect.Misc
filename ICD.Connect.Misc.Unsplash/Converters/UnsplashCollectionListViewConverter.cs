using System;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Misc.Unsplash.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash.Converters
{
	public sealed class UnsplashCollectionListViewConverter : AbstractUnsplashResponseConverter<UnsplashCollectionListViewResponse>
	{
		private const string ATTRIBUTE_TOTAL = "total";
		private const string ATTRIBUTE_TOTAL_PAGES = "total_pages";
		private const string ATTRIBUTE_RESULTS = "results";

		protected override void ReadProperty(string property, JsonReader reader, UnsplashCollectionListViewResponse instance,
		                                     JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_TOTAL:
					instance.Total = Convert.ToDouble(reader.GetValueAsString());
					break;
				case ATTRIBUTE_TOTAL_PAGES:
					instance.TotalPages = Convert.ToDouble(reader.GetValueAsString());
					break;
				case ATTRIBUTE_RESULTS:
					instance.Results = serializer.DeserializeArray<UnsplashCollectionResult>(reader).ToArray();
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}

	}
}
