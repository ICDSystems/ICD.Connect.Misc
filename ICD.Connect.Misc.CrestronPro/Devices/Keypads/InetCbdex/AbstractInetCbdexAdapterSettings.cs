using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.CrestronPro.Devices.Partitioning;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads
{
	public abstract class AbstractInetCbdexAdapterSettings : AbstractKeypadBaseAdapterSettings, IInetCbdexAdapterSettings
	{

		protected const string BARGRAPH_TIMEOUT_ELEMENT = "BargraphTimeout";
		protected const string HOLD_TIME_ELEMENT = "HoldTime";
		protected const string DOUBLE_TAP_SPEED_ELEMENT = "DoubleTapSpeed";
		protected const string WAIT_FOR_DOUBLE_TAP_ELEMENT = "WaitForDoubleTap";

		public ushort? BargraphTimeout { get; set; }
		public ushort? HoldTime { get; set; }
		public ushort? DoubleTapSpeed { get; set; }
		public bool? WaitForDoubleTap { get; set; }

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			BargraphTimeout = XmlUtils.TryReadChildElementContentAsUShort(xml, BARGRAPH_TIMEOUT_ELEMENT);
			HoldTime = XmlUtils.TryReadChildElementContentAsUShort(xml, HOLD_TIME_ELEMENT);
			DoubleTapSpeed = XmlUtils.TryReadChildElementContentAsUShort(xml, DOUBLE_TAP_SPEED_ELEMENT);
			WaitForDoubleTap = XmlUtils.TryReadChildElementContentAsBoolean(xml, WAIT_FOR_DOUBLE_TAP_ELEMENT);
		}
	}
}