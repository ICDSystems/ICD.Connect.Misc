using System;
using ICD.Connect.Misc.Unsplash_NetStandard.Converters;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash_NetStandard.Responses
{
	[JsonConverter(typeof(UnsplashPhotoListViewConverter))]
	public sealed class UnsplashPhotoListViewResponse : UnsplashAbstractResponse
	{
		public double Total { get; set; }
		public double TotalPages { get; set; }
		public UnsplashPhotoResponse[] Results { get; set; }
	}

	[JsonConverter(typeof(UnsplashPhotoResponseConverter))]
	public sealed class UnsplashPhotoResponse
	{
		public string Id { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public DateTime PromotedAt { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public string Color { get; set; }
		public string Description { get; set; }
		public string AltDescrption { get; set; }
		public UnsplashPhotoUrlResponse Urls { get; set; }
		public UnsplashPhotoLinkResponse Links { get; set; }
		public UnsplashPhotoCategoryResponse[] Categories { get; set; }
		public double Likes { get; set; }
		public bool LikesByUser { get; set; }
		public UnsplashPhotoUserResponse User { get; set; }
	}

	[JsonConverter(typeof(UnsplashPhotoUserConverter))]
	public sealed class UnsplashPhotoUserResponse
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Name { get; set; } 
	}

	[JsonConverter(typeof(UnsplashPhotoUrlConverter))]
	public sealed class UnsplashPhotoUrlResponse
	{
		public string Raw { get; set; }
		public string Full { get; set; }
		public string Regular { get; set; }
		public string Small { get; set; }
		public string Thumb { get; set; }
	}

	[JsonConverter(typeof(UnsplashPhotoLinkConverter))]
	public sealed class UnsplashPhotoLinkResponse
	{
		public Uri Self { get; set; }
		public Uri Html { get; set; }
		public Uri Download { get; set; }
		public Uri DownloadLocation { get; set; }
	}

	[JsonConverter(typeof(UnsplashPhotoCategoryConverter))]
	public sealed class UnsplashPhotoCategoryResponse
	{

	}
}
