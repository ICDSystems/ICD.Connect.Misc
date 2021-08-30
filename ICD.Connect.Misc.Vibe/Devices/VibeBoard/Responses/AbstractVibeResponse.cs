#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	public abstract class AbstractVibeResponse<TData> : IVibeResponse
	{
		public string Type { get; set; }

		public string ResultId { get; set; }

		public bool Sync { get; set; }

		public TData Value { get; set; }

		public string ErrorId { get; set; }

		public ErrorData Error { get; set; }
	}

	[JsonConverter(typeof(ErrorDataConverter))]
	public sealed class ErrorData
	{
		public string Code { get; set; }

		public string Message { get; set; }
	}
}
