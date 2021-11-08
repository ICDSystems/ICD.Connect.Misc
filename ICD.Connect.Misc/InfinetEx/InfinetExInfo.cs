using System;
using ICD.Common.Properties;

namespace ICD.Connect.Misc.InfinetEx
{
	/// <summary>
	/// Used in InfinetEx devices to hold InfinetEx information, and apply/copy/clear to settings
	/// </summary>
	public sealed class InfinetExInfo
	{
		private byte? m_RfId;
		private int? m_ParentId;

		#region Properties

		/// <summary>
		/// Gets the Cresnet ID
		/// </summary>
		public byte? RfId { get { return m_RfId; } }

		/// <summary>
		/// Gets the Parent ID
		/// </summary>
		public int? ParentId { get { return m_ParentId; } }

		#endregion

		#region Methods

		public void ApplySettings([NotNull] IInfinetExDeviceSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			m_RfId = settings.InfinetExSettings.RfId;
			m_ParentId = settings.InfinetExSettings.ParentId;
		}

		public void CopySettings([NotNull] IInfinetExDeviceSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			settings.InfinetExSettings.RfId = RfId;
			settings.InfinetExSettings.ParentId = ParentId;
		}

		public void ClearSettings()
		{
			m_RfId = null;
			m_ParentId = null;
		}

		#endregion
	}
}