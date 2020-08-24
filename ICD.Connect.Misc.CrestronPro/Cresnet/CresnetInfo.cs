using System;
using ICD.Common.Properties;

namespace ICD.Connect.Misc.CrestronPro.Cresnet
{
	public sealed class CresnetInfo
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
		public CresnetInfo()
		{
		}

		#endregion

		#region Methods

		public void ApplySettings([NotNull] ICresnetDeviceSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			m_CresnetId = settings.CresnetSettings.CresnetId;
			m_BranchId = settings.CresnetSettings.BranchId;
			m_ParentId = settings.CresnetSettings.ParentId;
		}

		public void CopySettings([NotNull] ICresnetDeviceSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			settings.CresnetSettings.CresnetId = CresnetId;
			settings.CresnetSettings.BranchId = BranchId;
			settings.CresnetSettings.ParentId = ParentId;
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