#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters
{
	public sealed class MuteResponseConverter : AbstractVibeResponseConverter<MuteResponse, MuteData>
	{
	}

	public sealed class MuteDataConverter : AbstractGenericJsonConverter<MuteData>
	{
		private const string PROP_IS_MUTE = "isMute";

		protected override void WriteProperties(JsonWriter writer, MuteData value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.IsMute)
				writer.WriteProperty(PROP_IS_MUTE, value.IsMute);
		}

		protected override void ReadProperty(string property, JsonReader reader, MuteData instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_IS_MUTE:
					instance.IsMute = reader.GetValueAsBool();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
