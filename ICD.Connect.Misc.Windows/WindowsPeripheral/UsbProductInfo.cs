using System;
using System.Text.RegularExpressions;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Misc.Windows.WindowsPeripheral
{
	public struct UsbProductInfo
	{
		public ushort VendorId { get; set; }
		public ushort ProductId { get; set; }
	}

	public static class UsbProductInfoExtensions
	{
		private const string VENDOR_ID_ELEMENT = "VendorId";
		private const string PRODUCT_ID_ELEMENT = "ProductId";

		private static readonly Regex s_DeviceIdRegex = new Regex(@"USB\\VID_([0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f])&PID_([0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f])\\([0-9A-Fa-f]+)");

		public static UsbProductInfo FromDeviceId(string deviceId)
		{
			var match = s_DeviceIdRegex.Match(deviceId);
			if (!match.Success)
				throw new ArgumentException(string.Format("DeviceId {0} is not a valid DeviceId format", deviceId),
				                            "deviceId");

			ushort vendorId = (ushort)Convert.ToInt64(match.Groups[1].Value, 16);
			ushort productId = (ushort)Convert.ToInt64(match.Groups[2].Value, 16);

			return new UsbProductInfo
			{
				VendorId = vendorId,
				ProductId = productId
			};
		}

		public static UsbProductInfo FromXml(string xml)
		{
			string vendorIdString = XmlUtils.TryReadChildElementContentAsString(xml, VENDOR_ID_ELEMENT);
			string productIdString = XmlUtils.TryReadChildElementContentAsString(xml, PRODUCT_ID_ELEMENT);

			ushort vendorId = (ushort)Convert.ToInt64(vendorIdString, 16);
			ushort productId = (ushort)Convert.ToInt64(productIdString, 16);

			return new UsbProductInfo
			{
				VendorId = vendorId,
				ProductId = productId
			};
		}
	}
}
