using System;
using ICD.Connect.Misc.Unsplash_NetStandard.Converters;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash_NetStandard.Responses
{
	[JsonConverter(typeof(UnsplashPhotoListViewConverter))]
	public sealed class UnsplashPhotoListViewResponse : AbstractUnsplashResponse
	{
		public double Total { get; set; }
		public double TotalPages { get; set; }
		public UnsplashPhotoResult[] Results { get; set; }
	}

	[JsonConverter(typeof(UnsplashPhotoResponseConverter))]
	public sealed class UnsplashPhotoResult
	{
		public string Id { get; set; }
		public string Width { get; set; }
		public string Height { get; set; }
		public string Color { get; set; }
		public string Description { get; set; }
		public string AltDescrption { get; set; }
		public UnsplashPhotoUrls Urls { get; set; }
		public UnsplashPhotoLinks Links { get; set; }
		public UnsplashPhotoCategory[] Categories { get; set; }
		public UnsplashPhotoUser User { get; set; }
	}

	[JsonConverter(typeof(UnsplashPhotoUserConverter))]
	public sealed class UnsplashPhotoUser
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Name { get; set; } 
	}

	[JsonConverter(typeof(UnsplashPhotoUrlConverter))]
	public sealed class UnsplashPhotoUrls
	{
		public string Raw { get; set; }
		public string Full { get; set; }
		public string Regular { get; set; }
		public string Small { get; set; }
		public string Thumb { get; set; }
	}

	[JsonConverter(typeof(UnsplashPhotoLinkConverter))]
	public sealed class UnsplashPhotoLinks
	{
		public string Self { get; set; }
		public string Html { get; set; }
		public string Download { get; set; }
		public string DownloadLocation { get; set; }
	}

	[JsonConverter(typeof(UnsplashPhotoCategoryConverter))]
	public sealed class UnsplashPhotoCategory
	{

	}
}
