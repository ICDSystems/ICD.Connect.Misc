using System;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class C3Com3AdapterSettings : AbstractC3CardAdapterSettings
	{
		private const string FACTORY_NAME = "C3Com3";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(C3Com3Adapter); } }
	}
}
