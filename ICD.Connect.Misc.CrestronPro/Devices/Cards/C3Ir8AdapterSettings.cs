using ICD.Common.Properties;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public sealed class C3Ir8AdapterSettings : AbstractCardAdapterSettings
	{
		private const string FACTORY_NAME = "C3Ir8";

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
			C3Ir8Adapter output = new C3Ir8Adapter();
			output.ApplySettings(this, factory);
			return output;
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
		public static C3Ir8AdapterSettings FromXml(string xml)
		{
			C3Ir8AdapterSettings output = new C3Ir8AdapterSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
