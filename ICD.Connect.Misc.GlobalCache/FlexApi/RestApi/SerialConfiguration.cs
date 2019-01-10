using Newtonsoft.Json;

namespace ICD.Connect.Misc.GlobalCache.FlexApi.RestApi
{
    public sealed class SerialConfiguration
    {
		public enum eParity
		{
			None = 0,
			Even = 1,
			Odd = 2
		}

		public enum eFlowControl
		{
			None = 0,
			Hardware = 1
		}

	    #region Properties

	    [JsonProperty("baudrate")]
	    public string BaudRate { get; set; }

		[JsonProperty("parity")]
	    public eParity Parity { get; set; }

	    [JsonProperty("flowcontrol")]
	    public eFlowControl FlowControl { get; set; }

	    #endregion
    }
}
