using ICD.Connect.Misc.Unsplash_NetStandard.Converters;
using ICD.Connect.Misc.Unsplash_NetStandard.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash
{
	[JsonConverter(typeof(UnsplashTokenResponseConverter))]
	public sealed class UnspashTokenResponse : UnsplashAbstractResponse
	{
	public string AccessToken { get; set; }
	public string TokenType { get; set; }
	public string RefreshToken { get; set; }
	public string Scope { get; set; }
	public string CreatedAt { get; set; }
	}
}
