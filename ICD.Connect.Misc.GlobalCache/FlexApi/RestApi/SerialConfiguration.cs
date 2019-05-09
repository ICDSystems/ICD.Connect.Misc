using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.GlobalCache.FlexApi.RestApi
{
	[JsonConverter(typeof(SerialConfigurationConverter))]
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

		public string BaudRate { get; set; }
		public eParity Parity { get; set; }
		public eFlowControl FlowControl { get; set; }
	}

	public sealed class SerialConfigurationConverter : AbstractGenericJsonConverter<SerialConfiguration>
	{
		private const string ATTR_BAUD_RATE = "baudrate";
		private const string ATTR_PARITY = "parity";
		private const string ATTR_FLOW_CONTROL = "flowcontrol";

		/// <summary>
		/// Override to write properties to the writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		/// <param name="serializer"></param>
		protected override void WriteProperties(JsonWriter writer, SerialConfiguration value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			writer.WriteProperty(ATTR_BAUD_RATE, value.BaudRate);
			writer.WriteProperty(ATTR_PARITY, value.Parity);
			writer.WriteProperty(ATTR_FLOW_CONTROL, value.FlowControl);
		}

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, SerialConfiguration instance,
		                                     JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTR_BAUD_RATE:
					instance.BaudRate = reader.GetValueAsString();
					break;

				case ATTR_PARITY:
					instance.Parity = reader.GetValueAsEnum<SerialConfiguration.eParity>();
					break;

				case ATTR_FLOW_CONTROL:
					instance.FlowControl = reader.GetValueAsEnum<SerialConfiguration.eFlowControl>();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
