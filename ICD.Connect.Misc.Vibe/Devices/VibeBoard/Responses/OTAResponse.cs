using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(OTAResponseConverter))]
	public sealed class OTAResponse : AbstractVibeResponse<SuccessData>
	{
	}
}
