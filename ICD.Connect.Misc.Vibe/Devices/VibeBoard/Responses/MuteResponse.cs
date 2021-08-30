#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(MuteResponseConverter))]
	public sealed class MuteResponse : AbstractVibeResponse<MuteData>
	{
	}

	[JsonConverter(typeof(MuteDataConverter))]
	public sealed class MuteData
	{
		public bool IsMute { get; set; }
	}
}
