using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using ICD.Common.Utils.IO;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.GlobalCache.FlexApi.RestApi
{
    public sealed class Module
    {
	    public enum eId
	    {
		    FlcIrSensor,
		    FlcSerial,
		    Flc3Emitter,
		    Flc2E1B,
		    FlcBlaster,
		    GlobalIrEmitter
	    }

	    public enum eType
	    {
		    FourRelayFourSensor,
		    Rs232,
		    Rs485,
		    TripleIrEmitter,
		    TwoIrEmitterOneIrBlaster,
		    OneBlaster,
		    OneEmitter
	    }

	    public enum eClass
	    {
			Infrared,
			Serial,
			RelaySensor
	    }

		private static readonly Dictionary<eId, string> s_IdStringMap = new Dictionary<eId, string>
		{
			{ eId.FlcIrSensor, "FLC-RS"},
			{ eId.FlcSerial, "FLC-SL"},
			{ eId.Flc3Emitter, "FLC-3E"},
			{ eId.Flc2E1B, "FLC-2E1B"},
			{ eId.FlcBlaster, "FLC-BL"},
			{ eId.GlobalIrEmitter, "G-IR-E"}
		};

	    private static readonly Dictionary<eType, string> s_TypeStringMap = new Dictionary<eType, string>
	    {
			{ eType.FourRelayFourSensor, "4R4S"},
		    { eType.Rs232, "RS232"},
		    { eType.Rs485, "RS485"},
		    { eType.TripleIrEmitter, "3Emitter"},
		    { eType.TwoIrEmitterOneIrBlaster, "2Emitter1Blaster"},
		    { eType.OneBlaster, "1Blaster"},
		    { eType.OneEmitter, "1Emitter"}
		};

	    private static readonly Dictionary<eClass, string> s_ClassStringMap = new Dictionary<eClass, string>
	    {
		    {eClass.Infrared, "Infrared"},
		    {eClass.RelaySensor, "Relay/Sensor"},
		    {eClass.Serial, "Serial"}
	    };

		public eId Id { get; set; }

		public eType Type { get; set; }

		public eClass Class { get; set; }

	    public string Serialize()
	    {
			StringBuilder builder = new StringBuilder();

		    using (IcdStringWriter writer = new IcdStringWriter(builder))
		    {
			    using (JsonWriter jsonWriter = new JsonTextWriter(writer.WrappedStringWriter))
			    {
				    jsonWriter.WriteStartObject();
				    {
					    jsonWriter.WritePropertyName("id");
					    jsonWriter.WriteValue(s_IdStringMap[Id]);

					    jsonWriter.WritePropertyName("class");
					    jsonWriter.WriteValue(s_ClassStringMap[Class]);

					    jsonWriter.WritePropertyName("type");
					    jsonWriter.WriteValue(s_TypeStringMap[Type]);
				    }
				    jsonWriter.WriteEndObject();

				    return builder.ToString();
			    }
		    }
		}
	}
}
