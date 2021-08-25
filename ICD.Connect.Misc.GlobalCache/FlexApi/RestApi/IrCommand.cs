#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Common.Utils.Json;

namespace ICD.Connect.Misc.GlobalCache.FlexApi.RestApi
{
	[JsonConverter(typeof(IrCommandConverter))]
	public sealed class IrCommand
	{
		public string Frequency { get; set; }
		public string Preamble { get; set; }
		public string IrCode { get; set; }
		public int Repeat { get; set; }
	}

	public sealed class IrCommandConverter : AbstractGenericJsonConverter<IrCommand>
	{
	}
}