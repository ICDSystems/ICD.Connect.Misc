using System;
using ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdPBaseWithVersiport;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdP
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class C2nCbdPAdapterSettings : AbstractC2nCbdPBaseWithVersiportAdapterSettings, IC2nCbdPAdapterSettings
	{
		private const string FACTORY_NAME = "C2nCbdP";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(C2nCbdPAdapter); } }
	}
}