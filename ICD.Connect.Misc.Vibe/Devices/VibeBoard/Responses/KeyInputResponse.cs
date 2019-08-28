using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(KeyInputResponseConverter))]
	public sealed class KeyInputResponse : AbstractVibeResponse<SuccessData>
	{
	}
}
