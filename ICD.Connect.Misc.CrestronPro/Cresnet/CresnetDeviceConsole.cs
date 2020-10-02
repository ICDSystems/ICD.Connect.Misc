using ICD.Common.Utils;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Misc.CrestronPro.Cresnet
{
	public static class CresnetDeviceConsole
	{
		public static void BuildConsoleStatus(ICresnetDevice instance, AddStatusRowDelegate addRow)
		{
			addRow("Cresnet ID", instance.CresnetInfo.CresnetId.HasValue
				                     ? StringUtils.ToIpIdString(instance.CresnetInfo.CresnetId.Value)
				                     : null);
			addRow("Parent ID", instance.CresnetInfo.ParentId);
			addRow("Branch ID", instance.CresnetInfo.BranchId);
		}
	}
}