using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Misc.CrestronPro.Utils
{
	public static class CresnetSettingsUtils
	{
		private const string CRESNET_ID_ELEMENT = "CresnetID";
		private const string BRANCH_ID_ELEMENT = "BranchID";
		private const string PARENT_ID_ELEMENT = "ParentID";

		#region Methods

		/// <summary>
		/// Reads Cresnet settings properties from the given XML and 
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="xml"></param>
		public static void ReadPropertiesFromXml(ICresnetDeviceSettings settings, string xml)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			settings.CresnetId = XmlUtils.TryReadChildElementContentAsByte(xml, CRESNET_ID_ELEMENT);
			settings.BranchId = XmlUtils.TryReadChildElementContentAsInt(xml, BRANCH_ID_ELEMENT);
			settings.ParentId = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_ID_ELEMENT);
		}

		public static void WritePropertiesToXml(ICresnetDeviceSettings settings, IcdXmlTextWriter writer)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteElementString(CRESNET_ID_ELEMENT, settings.CresnetId == null ? null : StringUtils.ToIpIdString(settings.CresnetId.Value));
			writer.WriteElementString(BRANCH_ID_ELEMENT, settings.BranchId == null ? null : settings.BranchId.Value.ToString());
			writer.WriteElementString(PARENT_ID_ELEMENT, settings.ParentId == null ? null : settings.ParentId.Value.ToString());
		}

		#endregion
	}
}