using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(ScreenResponseConverter))]
	public sealed class ScreenResponse : AbstractVibeResponse<ScreenData>
	{
	}

	[JsonConverter(typeof(ScreenDataConverter))]
	public sealed class ScreenData
	{
		public bool State { get; set; }

		public bool Success { get; set; }
	}
}
