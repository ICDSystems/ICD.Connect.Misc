using ICD.Common.Properties;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public sealed class C3Com3AdapterSettings : AbstractCardAdapterSettings
	{
		private const string FACTORY_NAME = "C3Com3";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Creates a new originator instance from the settings.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override IOriginator ToOriginator(IDeviceFactory factory)
		{
			C3Com3Adapter output = new C3Com3Adapter();
			output.ApplySettings(this, factory);
			return output;
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
		public static C3Com3AdapterSettings FromXml(string xml)
		{
			C3Com3AdapterSettings output = new C3Com3AdapterSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
