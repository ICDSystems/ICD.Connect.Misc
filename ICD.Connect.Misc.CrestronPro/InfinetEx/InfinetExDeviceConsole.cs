using ICD.Common.Utils;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Misc.CrestronPro.InfinetEx
{
	public static class InfinetExDeviceConsole
	{
		public static void BuildConsoleStatus(IInfinetExDevice instance, AddStatusRowDelegate addRow)
		{
			addRow("Cresnet ID", instance.InfinetExInfo.RfId.HasValue
									 ? StringUtils.ToIpIdString(instance.InfinetExInfo.RfId.Value)
									 : null);
			addRow("Parent ID", instance.InfinetExInfo.ParentId);
		}
	}
}