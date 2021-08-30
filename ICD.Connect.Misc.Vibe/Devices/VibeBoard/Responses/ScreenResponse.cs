#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;

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
