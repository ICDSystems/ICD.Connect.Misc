using ICD.Connect.Devices;

namespace ICD.Connect.Misc.CrestronPro
{
	public interface ICresnetDeviceSettings : IDeviceSettings
	{
		byte? CresnetId { get; set; }
		int? BranchId { get; set; }
		int? ParentId { get; set; }
	}
}