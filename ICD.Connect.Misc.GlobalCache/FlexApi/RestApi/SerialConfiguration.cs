using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.GlobalCache.FlexApi.RestApi
{
    public sealed class SerialConfiguration
    {
		public enum eGender
		{
			Male,
			Female
		}

		public enum eParity
		{
			None,
			Odd,
			Even
		}

		public enum eFlowControl
		{
			None,
			Hardware
		}

		public enum eDuplex
		{
			Half,
			Full
		}

	    #region Properties

	    public eGender Gender { get; set; }

		public int BaudRate { get; set; }

		public eParity Parity { get; set; }

		public int StopBits { get; set; }

		public eFlowControl FlowControl { get; set; }

		public eDuplex Duplex { get; set; }

	    #endregion

	    #region Methods

	    public string Serialize()
		{
			return JsonUtils.Serialize(Serialize);
		}

	    public void Serialize(JsonWriter writer)
	    {
			writer.WriteStartObject();
			{
				writer.WritePropertyName("gender");
				writer.WriteValue(Gender.ToString());

				writer.WritePropertyName("baudrate");
				writer.WriteValue(Gender.ToString());

				writer.WritePropertyName("parity");
				writer.WriteValue(Parity.ToString());

				writer.WritePropertyName("stopbits");
				writer.WriteValue(StopBits.ToString());

				writer.WritePropertyName("flowcontrol");
				writer.WriteValue(FlowControl.ToString());

				writer.WritePropertyName("duplex");
				writer.WriteValue(Duplex.ToString());
			}
			writer.WriteEndObject();
	    }

	    #endregion
    }
}
