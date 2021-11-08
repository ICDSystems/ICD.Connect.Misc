using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Misc.InfinetEx;

namespace ICD.Connect.Misc.CrestronPro.InfinetEx
{
	public abstract class AbstractInfinetExAdapterSettings : AbstractDeviceSettings, IInfinetExDeviceSettings
	{
		private readonly InfinetExSettings m_InfinetExSettings;

		/// <summary>
		/// Contains InfinetEx Settings Data
		/// </summary>
		public InfinetExSettings InfinetExSettings { get { return m_InfinetExSettings; } }

		/// <summary>
		/// Constructor
		/// </summary>
		protected AbstractInfinetExAdapterSettings()
		{
			m_InfinetExSettings = new InfinetExSettings();
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			InfinetExSettings.ParseXml(xml);
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			InfinetExSettings.WriteElements(writer);
		}
	}
}