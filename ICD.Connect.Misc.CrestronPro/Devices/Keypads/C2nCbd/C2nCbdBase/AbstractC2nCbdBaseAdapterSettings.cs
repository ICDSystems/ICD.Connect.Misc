using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.CrestronPro.Cresnet;
using ICD.Connect.Misc.CrestronPro.Devices.Keypads.InetCbdex;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdBase
{
	public abstract class AbstractC2nCbdBaseAdapterSettings : AbstractInetCbdexAdapterSettings, IC2nCbdBaseAdapterSettings
	{
		private readonly CresnetSettings m_CresnetSettings;

		public CresnetSettings CresnetSettings { get { return m_CresnetSettings; } }

		protected AbstractC2nCbdBaseAdapterSettings()
		{
			m_CresnetSettings = new CresnetSettings();
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			m_CresnetSettings.WriteElements(writer);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			m_CresnetSettings.ParseXml(xml);
		}
	}
}