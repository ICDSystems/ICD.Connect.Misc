using ICD.Connect.Devices;
using ICD.Common.Utils.Xml;
using ICD.Connect.Protocol.Network.Tcp;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	[KrangSettings("iTachFlex", typeof(GcITachFlexDevice))]
	public sealed class GcITachFlexDeviceSettings : AbstractDeviceSettings
	{
		private const string PORT_ELEMENT = "Port";

		/// <summary>
		/// The port id.
		/// </summary>
		[OriginatorIdSettingsProperty(typeof(AsyncTcpClient))]
		public int? Port { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);
		}
	}
}
