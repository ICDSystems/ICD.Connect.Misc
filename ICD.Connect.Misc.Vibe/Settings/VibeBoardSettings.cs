using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Panels.Server;
using ICD.Connect.Protocol.Network.Ports;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.Vibe.Settings
{
	[KrangSettings("VibeBoard", typeof(VibeBoard))]
	public class VibeBoardSettings : AbstractPanelServerDeviceSettings, INetworkProperties
	{
		private const string KRANG_PORT_ELEMENT = "KrangPort";

		private readonly SecureNetworkProperties m_NetworkProperties;

		#region Properties

		/// <summary>
		/// Krang Port originator id to connect to Vibe API
		/// </summary>
		[OriginatorIdSettingsProperty(typeof(ISecureNetworkPort))]
		public int? KrangPort { get; set; }

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
		public ushort? NetworkPort
		{
			get { return m_NetworkProperties.NetworkPort; }
			set { m_NetworkProperties.NetworkPort = value; }
		}

		/// <summary>
		/// Clears the configured values.
		/// </summary>
		void INetworkProperties.ClearNetworkProperties()
		{
			m_NetworkProperties.ClearNetworkProperties();
		}

		#endregion

		public VibeBoardSettings()
		{
			m_NetworkProperties = new SecureNetworkProperties();
		}
		
		/// <summary>
		/// Write settings elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(KRANG_PORT_ELEMENT, KrangPort == null ? null : IcdXmlConvert.ToString(KrangPort));

			m_NetworkProperties.WriteElements(writer);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			KrangPort = XmlUtils.TryReadChildElementContentAsInt(xml, KRANG_PORT_ELEMENT);

			m_NetworkProperties.ParseXml(xml);
			
			// apply defaults
			m_NetworkProperties.ApplyDefaultValues(null, 2222, "root", "123456");
		}
	}
}