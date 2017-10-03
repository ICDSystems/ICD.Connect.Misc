using System;

namespace ICD.Connect.Misc.GlobalCache.FlexApi
{
	public enum eHostType
	{
		Wifi,
		Ethernet
	}

	public enum eFlexLinkType
	{
		Ir,
		IrBlaster,
		IrTriport,
		IrTriportBlaster,
		Serial,
		RelaySensor,
		Sensor,
		SensorNotify
	}

	public enum eConfigLock
	{
		Locked,
		Unlocked
	}

	public enum eIpSetting
	{
		Dhcp,
		Static
	}

    public static class FlexUtils
	{
		/// <summary>
		/// Parses a version in the format 710-2000-15 
		/// </summary>
		/// <param name="flexVersion"></param>
		/// <returns></returns>
		public static Version ParseVersion(string flexVersion)
		{
			if (flexVersion == null)
				throw new ArgumentNullException("flexVersion");

			return new Version(flexVersion.Replace('-', '.'));
		}
	}
}
