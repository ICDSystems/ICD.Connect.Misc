using System.Text;
using ICD.Common.Utils.IO;
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

		public eGender Gender { get; set; }

		public int BaudRate { get; set; }

		public eParity Parity { get; set; }

		public int StopBits { get; set; }

		public eFlowControl FlowControl { get; set; }

		public eDuplex Duplex { get; set; }

		public string Serialize()
		{
			StringBuilder builder = new StringBuilder();

			using (IcdStringWriter writer = new IcdStringWriter(builder))
			{
				using (JsonWriter jsonWriter = new JsonTextWriter(writer.WrappedStringWriter))
				{
					jsonWriter.WriteStartObject();
					{
						jsonWriter.WritePropertyName("gender");
						jsonWriter.WriteValue(Gender.ToString());

						jsonWriter.WritePropertyName("baudrate");
						jsonWriter.WriteValue(Gender.ToString());

						jsonWriter.WritePropertyName("parity");
						jsonWriter.WriteValue(Parity.ToString());

						jsonWriter.WritePropertyName("stopbits");
						jsonWriter.WriteValue(StopBits.ToString());

						jsonWriter.WritePropertyName("flowcontrol");
						jsonWriter.WriteValue(FlowControl.ToString());

						jsonWriter.WritePropertyName("duplex");
						jsonWriter.WriteValue(Duplex.ToString());
					}
					jsonWriter.WriteEndObject();

					return builder.ToString();
				}
			}
		}
	}
}
