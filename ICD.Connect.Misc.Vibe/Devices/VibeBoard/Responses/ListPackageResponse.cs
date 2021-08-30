#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(ListPackageResponseConverter))]
	public sealed class ListPackageResponse : AbstractVibeResponse<PackageData[]>
	{
	}

	[JsonConverter(typeof(PackageDataConverter))]
	public sealed class PackageData
	{
		public string PackageName { get; set; }

		public bool IsSystem { get; set; }

		public string SourceDirectory { get; set; }
	}
}
