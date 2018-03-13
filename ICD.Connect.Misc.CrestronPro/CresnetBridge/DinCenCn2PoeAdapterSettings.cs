using System;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.CresnetBridge
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class DinCenCn2PoeAdapterSettings : AbstractDinCenCn2AdapterSettings
	{
		private const string FACTORY_NAME = "DinCenCn2Poe";
		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(DinCenCn2PoeAdapter); } }
	}
}