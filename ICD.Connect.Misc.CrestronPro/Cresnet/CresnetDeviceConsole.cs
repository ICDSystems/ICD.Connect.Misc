using ICD.Connect.API.Nodes;

namespace ICD.Connect.Misc.CrestronPro.Cresnet
{
	public static class CresnetDeviceConsole
	{
		public static void BuildConsoleStatus(ICresnetDevice instance, AddStatusRowDelegate addRow)
		{
			addRow("Cresnet ID", instance.CresnetDeviceInfo.CresnetId);
			addRow("Parent ID", instance.CresnetDeviceInfo.ParentId);
			addRow("Branch ID", instance.CresnetDeviceInfo.BranchId);
		}
	}
}