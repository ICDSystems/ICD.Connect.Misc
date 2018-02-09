using System;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.CardFrames
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class CenCi31AdapterSettings : AbstractCardFrameDeviceSettings
	{
		private const string FACTORY_NAME = "CenCi31";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(CenCi31Adapter); } }
	}
}
