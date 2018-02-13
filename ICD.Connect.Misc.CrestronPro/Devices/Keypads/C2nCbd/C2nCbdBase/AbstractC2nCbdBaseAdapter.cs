using System;
using Crestron.SimplSharpPro;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Misc.CrestronPro.Devices.Keypads.InetCbdex;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbd.C2nCbdBase
{
	public abstract class AbstractC2nCbdBaseAdapter<TKeypad, TSettings> : AbstractInetCbdexAdapter<TKeypad, TSettings>
		where TKeypad : Crestron.SimplSharpPro.Keypads.C2nCbdBase
		where TSettings : IC2nCbdBaseAdapterSettings, new()
	{
		protected abstract TKeypad InstantiateKeypad(byte cresnetId, CrestronControlSystem controlSystem);

		#region Settings

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);
#if SIMPLSHARP
			if (settings.CresnetId == null || !CresnetUtils.IsValidId(settings.CresnetId.Value))
			{
				Logger.AddEntry(eSeverity.Error, "{0} failed to instantiate {1} - CresnetId {2} is out of range",
								this, typeof(TKeypad).Name, settings.CresnetId);
				return;
			}

			TKeypad device = null;

			try
			{
				device = InstantiateKeypad((byte)settings.CresnetId, ProgramInfo.ControlSystem);
			}
			catch (ArgumentException e)
			{
				string message = string.Format("{0} failed to instantiate {1} with Cresnet ID {2} - {3}",
											   this, typeof(TKeypad).Name, settings.CresnetId, e.Message);
				Logger.AddEntry(eSeverity.Error, e, message);
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

			SetKeypad(null);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.CresnetId = Keypad == null ? (byte)0 : (byte)Keypad.ID;
		}

		#endregion
	}
}