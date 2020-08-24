using System;
using ICD.Common.Logging.LoggingContexts;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.CrestronPro.Cresnet;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro.DeviceSupport;
using ICD.Connect.Misc.CrestronPro.Utils;
#endif
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Misc.CrestronPro.Devices.Keypads.InetCbdex;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdBase
{
#if SIMPLSHARP
	public abstract class AbstractC2nCbdBaseAdapter<TKeypad, TSettings> : AbstractInetCbdexAdapter<TKeypad, TSettings>, ICresnetDevice
		where TKeypad : Crestron.SimplSharpPro.Keypads.C2nCbdBase
#else
	public abstract class AbstractC2nCbdBaseAdapter<TSettings> : AbstractInetCbdexAdapter<TSettings>
#endif
		where TSettings : IC2nCbdBaseAdapterSettings, new()
	{
#if SIMPLSHARP
		protected abstract TKeypad InstantiateKeypad(byte cresnetId);
		protected abstract TKeypad InstantiateKeypad(byte cresnetId, CresnetBranch branch);
#endif

		private readonly CresnetInfo m_CresnetInfo;

		public CresnetInfo CresnetInfo { get { return m_CresnetInfo; } }

		protected AbstractC2nCbdBaseAdapter()
		{
			m_CresnetInfo = new CresnetInfo();
		}

		#region Settings

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			CresnetInfo.ApplySettings(settings);

#if SIMPLSHARP
			if (m_CresnetInfo.CresnetId == null || !CresnetUtils.IsValidId(m_CresnetInfo.CresnetId.Value))
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} - CresnetId {1} is out of range",
						   typeof(TKeypad).Name, m_CresnetInfo.CresnetId);
				return;
			}

			TKeypad device = null;

			try
			{
				device = CresnetUtils.InstantiateCresnetDevice<TKeypad>(m_CresnetInfo.CresnetId.Value,
																		m_CresnetInfo.BranchId,
																		m_CresnetInfo.ParentId,
																		factory,
																		InstantiateKeypad,
																		InstantiateKeypad);
			}
			catch (ArgumentException e)
			{
				Logger.Log(eSeverity.Error, e, "Failed to instantiate {0} with Cresnet ID {1} - {2}",
						   typeof(TKeypad).Name, m_CresnetInfo.CresnetId, e.Message);
			}

			SetKeypad(device);
#endif
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			CresnetInfo.ClearSettings();

#if SIMPLSHARP
			SetKeypad(null);
#endif
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			CresnetInfo.CopySettings(settings);
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);
#if SIMPLSHARP
			CresnetDeviceConsole.BuildConsoleStatus(this, addRow);
#endif
		}

		#endregion
	}
}