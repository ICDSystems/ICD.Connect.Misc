using ICD.Connect.Misc.Unsplash_NetStandard.Responses;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Connect.Misc.Unsplash.Tests_NetStandard.converters
{
	[TestFixture]
	public sealed class UnsplashPhotoResponseConverterTest
	{
		[Test]
		public void DeserializeTest()
		{
			const string data = @"{""id"": ""7_TTPznVIQI"",
			""width"": 2588,
			""height"": 3456,
			""description"": ""Lilac-breasted Roller. I think the Lilac-breasted Roller is one of Africa’s most beautiful birds."",
			""alt_description"": ""shallow focus photography of multi colored bird"",
			""urls"": {
				""raw"": ""https://images.unsplash.com/photo-1535083783855-76ae62b2914e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEwNjczMn0"",
				""full"": ""https://images.unsplash.com/photo-1535083783855-76ae62b2914e?ixlib=rb-1.2.1&q=85&fm=jpg&crop=entropy&cs=srgb&ixid=eyJhcHBfaWQiOjEwNjczMn0"",
				""regular"": ""https://images.unsplash.com/photo-1535083783855-76ae62b2914e?ixlib=rb-1.2.1&q=80&fm=jpg&crop=entropy&cs=tinysrgb&w=1080&fit=max&ixid=eyJhcHBfaWQiOjEwNjczMn0"",
				""small"": ""https://images.unsplash.com/photo-1535083783855-76ae62b2914e?ixlib=rb-1.2.1&q=80&fm=jpg&crop=entropy&cs=tinysrgb&w=400&fit=max&ixid=eyJhcHBfaWQiOjEwNjczMn0"",
				""thumb"": ""https://images.unsplash.com/photo-1535083783855-76ae62b2914e?ixlib=rb-1.2.1&q=80&fm=jpg&crop=entropy&cs=tinysrgb&w=200&fit=max&ixid=eyJhcHBfaWQiOjEwNjczMn0""

			},
			""links"": {
				""self"": ""https://api.unsplash.com/photos/7_TTPznVIQI"",
				""html"": ""https://unsplash.com/photos/7_TTPznVIQI"",
				""download"": ""https://unsplash.com/photos/7_TTPznVIQI/download"",
				""download_location"": ""https://api.unsplash.com/photos/7_TTPznVIQI/download""

			},
			""categories"": [],
			""user"": {
				""id"": ""TYLyWjPA9BM"",
				""updated_at"": ""2019-12-16T16:20:22-05:00"",
				""username"": ""davidclode"",
				""name"": ""David Clode"",
			}
		}";

			UnsplashPhotoResult photo = JsonConvert.DeserializeObject<UnsplashPhotoResult>(data);

			Assert.AreEqual("7_TTPznVIQI", photo.Id);
			Assert.AreEqual("2588", photo.Width);
			Assert.AreEqual("3456", photo.Height);
			Assert.AreEqual("Lilac-breasted Roller. I think the Lilac-breasted Roller is one of Africa’s most beautiful birds.", photo.Description);
			Assert.AreEqual("shallow focus photography of multi colored bird", photo.AltDescrption);
			Assert.AreEqual("https://images.unsplash.com/photo-1535083783855-76ae62b2914e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEwNjczMn0", photo.Urls.Raw);
			Assert.AreEqual("https://images.unsplash.com/photo-1535083783855-76ae62b2914e?ixlib=rb-1.2.1&q=85&fm=jpg&crop=entropy&cs=srgb&ixid=eyJhcHBfaWQiOjEwNjczMn0", photo.Urls.Full);
			Assert.AreEqual("https://images.unsplash.com/photo-1535083783855-76ae62b2914e?ixlib=rb-1.2.1&q=80&fm=jpg&crop=entropy&cs=tinysrgb&w=1080&fit=max&ixid=eyJhcHBfaWQiOjEwNjczMn0", photo.Urls.Regular);
			Assert.AreEqual("https://images.unsplash.com/photo-1535083783855-76ae62b2914e?ixlib=rb-1.2.1&q=80&fm=jpg&crop=entropy&cs=tinysrgb&w=400&fit=max&ixid=eyJhcHBfaWQiOjEwNjczMn0", photo.Urls.Small);
			Assert.AreEqual("https://images.unsplash.com/photo-1535083783855-76ae62b2914e?ixlib=rb-1.2.1&q=80&fm=jpg&crop=entropy&cs=tinysrgb&w=200&fit=max&ixid=eyJhcHBfaWQiOjEwNjczMn0", photo.Urls.Thumb);
			Assert.AreEqual("https://api.unsplash.com/photos/7_TTPznVIQI", photo.Links.Self);
			Assert.AreEqual("https://unsplash.com/photos/7_TTPznVIQI", photo.Links.Html);
			Assert.AreEqual("https://unsplash.com/photos/7_TTPznVIQI/download", photo.Links.Download);
			Assert.AreEqual("https://api.unsplash.com/photos/7_TTPznVIQI/download", photo.Links.DownloadLocation);
			Assert.AreEqual(0, photo.Categories.Length);
			Assert.AreEqual("TYLyWjPA9BM", photo.User.UserId);
			Assert.AreEqual("davidclode", photo.User.UserName);
			Assert.AreEqual("David Clode", photo.User.Name);
		}
	}
}