#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters
{
	public sealed class DumpResponseConverter : AbstractVibeResponseConverter<DumpResponse, UsbDeviceData[]>
	{
	}

	public sealed class UsbDeviceDataConverter : AbstractGenericJsonConverter<UsbDeviceData>
	{
		private const string PROP_DEVICE_NAME = "deviceName";
		private const string PROP_MANUFACTURER_NAME = "manufacturerName";
		private const string PROP_PRODUCT_NAME = "productName";
		private const string PROP_VERSION = "version";
		private const string PROP_SERIAL_NUMBER = "serialNumber";
		private const string PROP_DEVICE_ID = "deviceId";
		private const string PROP_VENDOR_ID = "vendorId";
		private const string PROP_PRODUCT_ID = "productId";
		private const string PROP_DEVICE_CLASS = "deviceClass";
		private const string PROP_DEVICE_SUBCLASS = "deviceSubclass";
		private const string PROP_PROTOCOL = "protocol";

		protected override void WriteProperties(JsonWriter writer, UsbDeviceData value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.DeviceName != null)
				writer.WriteProperty(PROP_DEVICE_NAME, value.DeviceName);

			if (value.ManufacturerName != null)
				writer.WriteProperty(PROP_MANUFACTURER_NAME, value.ManufacturerName);

			if (value.ProductName != null)
				writer.WriteProperty(PROP_PRODUCT_NAME, value.ProductName);

			if (value.Version != null)
				writer.WriteProperty(PROP_VERSION, value.Version);

			if (value.SerialNumber != null)
				writer.WriteProperty(PROP_SERIAL_NUMBER, value.SerialNumber);

			if (value.DeviceId != 0)
				writer.WriteProperty(PROP_DEVICE_ID, value.DeviceId);

			if (value.VendorId != 0)
				writer.WriteProperty(PROP_VENDOR_ID, value.VendorId);

			if (value.ProductId != 0)
				writer.WriteProperty(PROP_PRODUCT_ID, value.ProductId);

			if (value.DeviceClass != 0)
				writer.WriteProperty(PROP_DEVICE_CLASS, value.DeviceClass);

			if (value.DeviceSubclass != 0)
				writer.WriteProperty(PROP_DEVICE_SUBCLASS, value.DeviceSubclass);

			if (value.Protocol != 0)
				writer.WriteProperty(PROP_PROTOCOL, value.Protocol);
		}

		protected override void ReadProperty(string property, JsonReader reader, UsbDeviceData instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_DEVICE_NAME:
					instance.DeviceName = reader.GetValueAsString();
					break;

				case PROP_MANUFACTURER_NAME:
					instance.ManufacturerName = reader.GetValueAsString();
					break;

				case PROP_PRODUCT_NAME:
					instance.ProductName = reader.GetValueAsString();
					break;

				case PROP_VERSION:
					instance.Version = reader.GetValueAsString();
					break;

				case PROP_SERIAL_NUMBER:
					instance.SerialNumber = reader.GetValueAsString();
					break;

				case PROP_DEVICE_ID:
					instance.DeviceId = reader.GetValueAsInt();
					break;

				case PROP_VENDOR_ID:
					instance.VendorId = reader.GetValueAsInt();
					break;

				case PROP_PRODUCT_ID:
					instance.ProductId = reader.GetValueAsInt();
					break;

				case PROP_DEVICE_CLASS:
					instance.DeviceClass = reader.GetValueAsInt();
					break;

				case PROP_DEVICE_SUBCLASS:
					instance.DeviceSubclass = reader.GetValueAsInt();
					break;

				case PROP_PROTOCOL:
					instance.Protocol = reader.GetValueAsInt();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
