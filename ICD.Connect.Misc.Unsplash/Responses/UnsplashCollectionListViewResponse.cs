using System;
using ICD.Connect.Misc.Unsplash.Converters;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash.Responses
{
	[JsonConverter(typeof(UnsplashCollectionListViewConverter))]
	public sealed class UnsplashCollectionListViewResponse : AbstractUnsplashResponse
	{
		public double Total { get; set; }
		public double TotalPages { get; set; }
		public UnsplashCollectionResult[] Results { get; set; }

	}

	[JsonConverter(typeof(UnsplashCollectionResultConverter))]
	public sealed class UnsplashCollectionResult
	{
		public double Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public bool Curated { get; set; }
		public bool Features { get; set; }
		public double TotalPhotos { get; set; }
		public bool Private { get; set; }
		public string ShareKey { get; set; }
		public UnsplashCollectionLinks Links { get; set; }
		public UnsplashCollectionUser User { get; set; } 
	}

	[JsonConverter(typeof(UnsplashCollectionUserConverter))]
	public sealed class UnsplashCollectionUser
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Name { get; set; }
	}

	[JsonConverter(typeof(UnsplashCollectionLinksConverter))]
	public sealed class UnsplashCollectionLinks
	{
		public string Self { get; set; }
		public string Html { get; set; }
		public string Photos { get; set; }
		public string Related { get; set; }
	}
}
