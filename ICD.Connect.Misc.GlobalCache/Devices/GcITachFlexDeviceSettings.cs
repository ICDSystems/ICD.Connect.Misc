using ICD.Connect.Devices;
using ICD.Common.Utils.Xml;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	[KrangSettings("iTachFlex", typeof(GcITachFlexDevice))]
	public sealed class GcITachFlexDeviceSettings : AbstractDeviceSettings, IUriSettings, ISecureNetworkSettings
	{
		private const ushort DEFAULT_NETWORK_PORT = 4998;

		private const string SERIAL_PORT_ELEMENT = "SerialPort";
		private const string WEB_PORT_ELEMENT = "WebPort";

		private readonly UriProperties m_UriProperties;
		private readonly SecureNetworkProperties m_NetworkProperties;

		#region Properties

		/// <summary>
		/// The serial port id.
		/// </summary>
		[OriginatorIdSettingsProperty(typeof(ISerialPort))]
		public int? SerialPort { get; set; }

		/// <summary>
		/// The web port id.
		/// </summary>
		[OriginatorIdSettingsProperty(typeof(IWebPort))]
		public int? WebPort { get; set; }

		#endregion

		#region Network

		/// <summary>
		/// Gets/sets the configurable network username.
		/// </summary>
		public string NetworkUsername
		{
			get { return m_NetworkProperties.NetworkUsername; }
			set { m_NetworkProperties.NetworkUsername = value; }
		}

		/// <summary>
		/// Gets/sets the configurable network password.
		/// </summary>
		public string NetworkPassword
		{
			get { return m_NetworkProperties.NetworkPassword; }
			set { m_NetworkProperties.NetworkPassword = value; }
		}

		/// <summary>
		/// Gets/sets the configurable network address.
		/// </summary>
		public string NetworkAddress
		{
			get { return m_NetworkProperties.NetworkAddress; }
			set { m_NetworkProperties.NetworkAddress = value; }
		}

		/// <summary>
		/// Gets/sets the configurable network port.
		/// </summary>
		public ushort NetworkPort
		{
			get { return m_NetworkProperties.NetworkPort; }
			set { m_NetworkProperties.NetworkPort = value; }
		}

		#endregion

		#region URI

		/// <summary>
		/// Gets/sets the configurable URI username.
		/// </summary>
		public string UriUsername { get { return m_UriProperties.UriUsername; } set { m_UriProperties.UriUsername = value; } }

		/// <summary>
		/// Gets/sets the configurable URI password.
		/// </summary>
		public string UriPassword { get { return m_UriProperties.UriPassword; } set { m_UriProperties.UriPassword = value; } }

		/// <summary>
		/// Gets/sets the configurable URI host.
		/// </summary>
		public string UriHost { get { return m_UriProperties.UriHost; } set { m_UriProperties.UriHost = value; } }

		/// <summary>
		/// Gets/sets the configurable URI port.
		/// </summary>
		public ushort UriPort { get { return m_UriProperties.UriPort; } set { m_UriProperties.UriPort = value; } }

		/// <summary>
		/// Gets/sets the configurable URI scheme.
		/// </summary>
		public string UriScheme { get { return m_UriProperties.UriScheme; } set { m_UriProperties.UriScheme = value; } }

		/// <summary>
		/// Gets/sets the configurable URI path.
		/// </summary>
		public string UriPath { get { return m_UriProperties.UriPath; } set { m_UriProperties.UriPath = value; } }

		/// <summary>
		/// Gets/sets the configurable URI query.
		/// </summary>
		public string UriQuery { get { return m_UriProperties.UriQuery; } set { m_UriProperties.UriQuery = value; } }

		/// <summary>
		/// Gets/sets the configurable URI fragment.
		/// </summary>
		public string UriFragment { get { return m_UriProperties.UriFragment; } set { m_UriProperties.UriFragment = value; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachFlexDeviceSettings()
		{
			m_UriProperties = new UriProperties();

			m_NetworkProperties = new SecureNetworkProperties
			{
				NetworkPort = DEFAULT_NETWORK_PORT
			};
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(WEB_PORT_ELEMENT, IcdXmlConvert.ToString(WebPort));
			writer.WriteElementString(SERIAL_PORT_ELEMENT, IcdXmlConvert.ToString(SerialPort));

			m_UriProperties.WriteElements(writer);
			m_NetworkProperties.WriteElements(writer);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			WebPort = XmlUtils.TryReadChildElementContentAsInt(xml, WEB_PORT_ELEMENT);
			SerialPort = XmlUtils.TryReadChildElementContentAsInt(xml, SERIAL_PORT_ELEMENT);

			m_UriProperties.ParseXml(xml);
			m_NetworkProperties.ParseXml(xml);
		}
	}
}
