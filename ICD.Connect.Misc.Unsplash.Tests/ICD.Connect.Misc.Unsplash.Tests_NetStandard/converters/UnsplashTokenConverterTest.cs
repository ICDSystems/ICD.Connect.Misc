using Newtonsoft.Json;
using NUnit.Framework;

namespace ICD.Connect.Misc.Unsplash.Tests_NetStandard
{
	[TestFixture]
	public sealed class UnsplashTokenConverterTest
	{
		[Test]
		public void DeserializeTest()
		{
			const string token = @"{
			""access_token"": ""8f2f469c0af19cf6af4d09910b645790b750b5cffba0366e1b6b5498287ef0ac"",
			""token_type"": ""Bearer"",
			""refresh_token"": ""5fda786677d1abcfea40ee40fd53cddeea532da6a55591d977066011c9ebe0fc"",
			""scope"": ""public read_user"",
			""created_at"": 1576869809
		}";

			UnspashTokenResponse response = JsonConvert.DeserializeObject<UnspashTokenResponse>(token);

			Assert.AreEqual("8f2f469c0af19cf6af4d09910b645790b750b5cffba0366e1b6b5498287ef0ac", response.AccessToken);
			Assert.AreEqual("Bearer", response.TokenType);
			Assert.AreEqual("5fda786677d1abcfea40ee40fd53cddeea532da6a55591d977066011c9ebe0fc", response.RefreshToken);
			Assert.AreEqual("public read_user", response.Scope);
			Assert.AreEqual("1576869809", response.CreatedAt);
		}
	}
}
