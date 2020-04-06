using ICD.Common.Utils.Xml;
using ICD.Connect.Devices.Mock;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.Keypads.MockKeypad
{
	[KrangSettings("MockKeypad", typeof(MockKeypad))]
	public sealed class MockKeypadSettings : AbstractKeypadDeviceSettings, IMockKeypadSettings
	{
		public bool DefaultOffline { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			MockDeviceSettingsHelper.WriteElements(this, writer);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			MockDeviceSettingsHelper.ParseXml(this, xml);
		}
	}
}
