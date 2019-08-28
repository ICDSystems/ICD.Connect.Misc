using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters
{
	public sealed class VolumeResponseConverter : AbstractVibeResponseConverter<VolumeResponse, VolumeData>
	{
	}

	public sealed class VolumeDataConverter : AbstractGenericJsonConverter<VolumeData>
	{
		private const string PROP_VOLUME = "volume";

		protected override void WriteProperties(JsonWriter writer, VolumeData value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.Volume != default(int))
				writer.WriteProperty(PROP_VOLUME, value.Volume);
		}

		protected override void ReadProperty(string property, JsonReader reader, VolumeData instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_VOLUME:
					instance.Volume = reader.GetValueAsInt();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
