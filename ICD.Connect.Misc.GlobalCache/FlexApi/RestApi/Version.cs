using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.GlobalCache.FlexApi.RestApi
{
	[JsonConverter(typeof(VersionConverter))]
	public sealed class Version
	{
		 public string Make { get; set; }
		 public string Model { get; set; }
		 public string Host { get; set; }
		 public string FirmwareVersion { get; set; }
		 public string UserName { get; set; }
		 public string Password { get; set; }
		 public string BoardVersion { get; set; }
}

	public sealed class VersionConverter : AbstractGenericJsonConverter<Version>
	{
		private const string ATTR_MAKE = "make";
		private const string ATTR_MODEL = "model";
		private const string ATTR_HOST = "host";
		private const string ATTR_FIRMWARE_VERSION = "firmwareVersion";
		private const string ATTR_USER_NAME = "userName";
		private const string ATTR_PASSWORD = "password";
		private const string ATTR_BOARD_VERSION = "boardVersion";

		/// <summary>
		/// Override to write properties to the writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		/// <param name="serializer"></param>
		protected override void WriteProperties(JsonWriter writer, Version value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			writer.WriteProperty(ATTR_MAKE, value.Make);
			writer.WriteProperty(ATTR_MODEL, value.Model);
			writer.WriteProperty(ATTR_HOST, value.Host);
			writer.WriteProperty(ATTR_FIRMWARE_VERSION, value.FirmwareVersion);
			writer.WriteProperty(ATTR_USER_NAME, value.UserName);
			writer.WriteProperty(ATTR_PASSWORD, value.Password);
			writer.WriteProperty(ATTR_BOARD_VERSION, value.BoardVersion);
		}

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, Version instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTR_MAKE:
					instance.Make = reader.GetValueAsString();
					break;
				case ATTR_MODEL:
					instance.Model = reader.GetValueAsString();
					break;
				case ATTR_HOST:
					instance.Host = reader.GetValueAsString();
					break;
				case ATTR_FIRMWARE_VERSION:
					instance.FirmwareVersion = reader.GetValueAsString();
					break;
				case ATTR_USER_NAME:
					instance.UserName = reader.GetValueAsString();
					break;
				case ATTR_PASSWORD:
					instance.Password = reader.GetValueAsString();
					break;
				case ATTR_BOARD_VERSION:
					instance.BoardVersion = reader.GetValueAsString();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}