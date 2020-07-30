namespace ICD.Connect.Misc.CrestronPro.Cresnet
{
	public sealed class CresnetDeviceInfo
	{
		private byte? m_CresnetId;
		private int? m_BranchId;
		private int? m_ParentId;

		#region Properties

		/// <summary>
		/// Gets the Cresnet ID
		/// </summary>
		public byte? CresnetId { get { return m_CresnetId; } }
		
		/// <summary>
		/// Gets the Branch ID
		/// </summary>
		public int? BranchId { get { return m_BranchId; }}
		
		/// <summary>
		/// Gets the Parent ID
		/// </summary>
		public int? ParentId { get { return m_ParentId; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="settings"></param>
		public CresnetDeviceInfo(ICresnetDeviceSettings settings)
		{
			m_CresnetId = settings.CresnetDeviceSettings.CresnetId;
			m_BranchId = settings.CresnetDeviceSettings.BranchId;
			m_ParentId = settings.CresnetDeviceSettings.ParentId;
		}

		#endregion

		#region Methods

		public void CopySettings(ICresnetDeviceSettings settings)
		{
			settings.CresnetDeviceSettings.CresnetId = CresnetId;
			settings.CresnetDeviceSettings.BranchId = BranchId;
			settings.CresnetDeviceSettings.ParentId = ParentId;
		}

		public void ClearSettings()
		{
			m_CresnetId = null;
			m_BranchId = null;
			m_ParentId = null;
		}

		#endregion
	}
}