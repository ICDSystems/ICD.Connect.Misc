using ICD.Connect.Devices;
using ICD.Common.Utils.Xml;
﻿using ICD.Connect.Protocol.Network.Ports.Tcp;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	public abstract class AbstractGcITachDeviceSettings : AbstractDeviceSettings, IGcITachDeviceSettings
	{
		private const string PORT_ELEMENT = "Port";

		private readonly NetworkProperties m_NetworkProperties;

		#region Properties

		/// <summary>
		/// The port id.
		/// </summary>
		[OriginatorIdSettingsProperty(typeof(AsyncTcpClient))]
		public int? Port { get; set; }

		#endregion

		#region Network

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
		void INetworkProperties.Clear()
		{
			m_NetworkProperties.Clear();
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractGcITachDeviceSettings()
		{
			m_NetworkProperties = new NetworkProperties();
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));

			m_NetworkProperties.WriteElements(writer);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);

			m_NetworkProperties.ParseXml(xml);
		}
	}
}
