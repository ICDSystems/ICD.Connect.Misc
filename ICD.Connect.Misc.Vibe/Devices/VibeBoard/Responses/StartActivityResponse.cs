using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(StartActivityResponseConverter))]
	public sealed class StartActivityResponse : AbstractVibeResponse<SuccessData>
	{
	}
}
