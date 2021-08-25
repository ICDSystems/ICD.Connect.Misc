#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Connect.Misc.Unsplash.Responses;
using NUnit.Framework;

namespace ICD.Connect.Misc.Unsplash.Tests.Converters
{
	[TestFixture]
	public sealed class UnsplashCollectionResponseConverterTest
	{
		[Test]
		public void DeserializeTest()
		{
			const string data = @"{""id"": 1424240,
			""title"": ""Animals"",
            ""description"": ""Man gave names to all the animals\\nIn the beginning, in the beginning.\\nMan gave names to all the animals\\nIn the beginning, long time ago.\\n- Bob Dylan"",
            ""curated"": false,
            ""featured"": true,
            ""total_photos"": 811,
            ""private"": false,
            ""share_key"": ""8ed7d1eb1cc0019e651134cef0956d08"",
            ""links"": {
                ""self"": ""https://api.unsplash.com/collections/1424240"",
                ""html"": ""https://unsplash.com/collections/1424240/animals"",
                ""photos"": ""https://api.unsplash.com/collections/1424240/photos"",
                ""related"": ""https://api.unsplash.com/collections/1424240/related""
            },
            ""user"": {
				""id"": ""gUgi6ncPlWg"",
				""username"": ""wsanter"",
				""name"": ""Wilfried Santer"",
			}
		}";

			UnsplashCollectionResult collection = JsonConvert.DeserializeObject<UnsplashCollectionResult>(data);

			Assert.AreEqual(1424240, collection.Id);
			Assert.AreEqual("Animals", collection.Title);
			Assert.AreEqual("Man gave names to all the animals\\nIn the beginning, in the beginning.\\nMan gave names to all the animals\\nIn the beginning, long time ago.\\n- Bob Dylan", collection.Description);
			Assert.AreEqual(false, collection.Curated);
			Assert.AreEqual(true, collection.Features);
			Assert.AreEqual(811, collection.TotalPhotos);
			Assert.AreEqual(false, collection.Private);
			Assert.AreEqual("8ed7d1eb1cc0019e651134cef0956d08", collection.ShareKey);
			Assert.AreEqual("https://api.unsplash.com/collections/1424240", collection.Links.Self);
			Assert.AreEqual("https://unsplash.com/collections/1424240/animals", collection.Links.Html);
			Assert.AreEqual("https://api.unsplash.com/collections/1424240/photos", collection.Links.Photos);
			Assert.AreEqual("https://api.unsplash.com/collections/1424240/related", collection.Links.Related);
			Assert.AreEqual("gUgi6ncPlWg", collection.User.UserId);
			Assert.AreEqual("wsanter", collection.User.UserName);
			Assert.AreEqual("Wilfried Santer", collection.User.Name);
		}
	}
}
