#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(TaskSwitchResponseConverter))]
	public sealed class TaskSwitchResponse : AbstractVibeResponse<SuccessData>
	{
	}
}
