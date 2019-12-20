using ICD.Common.Utils.Extensions;
using ICD.Connect.Misc.Unsplash;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash_NetStandard.Converters
{
	public sealed class UnsplashTokenResponseConverter: UnsplashAbstractResponseConverter<UnspashTokenResponse>
	{
		private const string ATTRIBUTE_ACCESS_TOKEN = "access_token";
		private const string ATTRIBUTE_TOKEN_TYPE = "token_type";
		private const string ATTRIBUTE_REFRESH_TOKEN = "refresh_token";
		private const string ATTRIBUTE_SCOPE = "scope";
		private const string ATTRIBUTE_CREATED_AT = "created_at";

		protected override void ReadProperty(string property, JsonReader reader, UnspashTokenResponse instance,
		                                     JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTRIBUTE_ACCESS_TOKEN:
					instance.AccessToken = reader.GetValueAsString();
					break;
				case ATTRIBUTE_TOKEN_TYPE:
					instance.TokenType = reader.GetValueAsString();
					break;
				case ATTRIBUTE_REFRESH_TOKEN:
					instance.RefreshToken = reader.GetValueAsString();
					break;
				case ATTRIBUTE_SCOPE:
					instance.Scope = reader.GetValueAsString();
					break;
				case ATTRIBUTE_CREATED_AT:
					instance.CreatedAt = reader.GetValueAsString();
					break;
				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;

			}

		}
	}
}
