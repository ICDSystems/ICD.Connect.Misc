using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(VolumeResponseConverter))]
	public sealed class VolumeResponse : AbstractVibeResponse<VolumeData>
	{
	}

	[JsonConverter(typeof(VolumeDataConverter))]
	public class VolumeData
	{
		public int Volume { get; set; }
	}
}
