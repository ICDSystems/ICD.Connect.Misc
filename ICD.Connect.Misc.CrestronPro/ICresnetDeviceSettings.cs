namespace ICD.Connect.Misc.CrestronPro
{
	public interface ICresnetDeviceSettings
	{
		byte? CresnetId { get; set; }
		int? BranchId { get; set; }
		int? ParentId { get; set; }
	}
}