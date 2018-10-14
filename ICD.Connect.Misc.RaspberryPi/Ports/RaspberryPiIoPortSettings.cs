using ICD.Common.Utils.Xml;
using ICD.Connect.Protocol.Ports.IoPort;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.RaspberryPi.Ports
{
	[KrangSettings("RaspberryPiIoPort", typeof(RaspberryPiIoPort))]
	public sealed class RaspberryPiIoPortSettings : AbstractIoPortSettings
	{
		private const string PIN_ELEMENT = "Pin";

		public int Pin { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PIN_ELEMENT, IcdXmlConvert.ToString(Pin));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Pin = XmlUtils.TryReadChildElementContentAsInt(xml, PIN_ELEMENT) ?? 0;
		}
	}
}
