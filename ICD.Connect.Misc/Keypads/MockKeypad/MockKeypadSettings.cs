using System;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.Keypads.MockKeypad
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class MockKeypadSettings : AbstractKeypadDeviceSettings, IMockKeypadSettings
	{
		private const string FACTORY_NAME = "MockKeypad";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(MockKeypad); } }
	}
}