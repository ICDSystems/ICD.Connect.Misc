﻿using System;
using ICD.Connect.Devices;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
	[KrangSettings(FACTORY_NAME)]
    public sealed class GcITachFlexDeviceSettings : AbstractDeviceSettings
	{
		private const string FACTORY_NAME = "iTachFlex";

		private const string ADDRESS_ELEMENT = "Address";

		public override string FactoryName { get { return FACTORY_NAME; } }

		public override Type OriginatorType { get { return typeof(GcITachFlexDevice); } }

		/// <summary>
		/// The network address of the iTach device.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(ADDRESS_ELEMENT, Address);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Address = XmlUtils.TryReadChildElementContentAsString(xml, ADDRESS_ELEMENT);
		}
	}
}