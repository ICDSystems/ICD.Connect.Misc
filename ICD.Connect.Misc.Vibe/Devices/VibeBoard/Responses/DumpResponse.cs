using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	[JsonConverter(typeof(DumpResponseConverter))]
	public sealed class DumpResponse : AbstractVibeResponse<UsbDeviceData[]>
	{
	}

	[JsonConverter(typeof(UsbDeviceDataConverter))]
	public sealed class UsbDeviceData
	{
		public string DeviceName { get; set; }

		public string ManufacturerName { get; set; }

		public string ProductName { get; set; }

		public string Version { get; set; }

		public string SerialNumber { get; set; }

		public int DeviceId { get; set; }

		public int VendorId { get; set; }

		public int ProductId { get; set; }

		public int DeviceClass { get; set; }

		public int DeviceSubclass { get; set; }

		public int Protocol { get; set; }
	}
}
