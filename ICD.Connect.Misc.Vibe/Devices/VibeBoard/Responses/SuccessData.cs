using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(SuccessDataConverter))]
	public sealed class SuccessData
	{
		public bool Success { get; set; }
	}
}
