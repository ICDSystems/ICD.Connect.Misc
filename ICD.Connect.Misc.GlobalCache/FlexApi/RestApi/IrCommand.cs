using ICD.Common.Utils.Json;
using Newtonsoft.Json;

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