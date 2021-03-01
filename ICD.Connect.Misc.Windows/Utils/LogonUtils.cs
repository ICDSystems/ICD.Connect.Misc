#if !SIMPLSHARP
using System;
using ICD.Common.Utils;
using Microsoft.Win32;

namespace ICD.Connect.Misc.Windows.Utils
{
	public static class LogonUtils
	{
		/// <summary>
		/// Restarts the computer and logs into the given account automatically.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		public static void SwitchUser(string username, string password)
		{
			SetDefaultLogon(username, password);
			ProcessorUtils.Reboot();
		}

		/// <summary>
		/// Sets the default windows logon account.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		private static void SetDefaultLogon(string username, string password)
		{
			const string key = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key, true) ?? Registry.LocalMachine.CreateSubKey(key);
			if (registryKey == null)
				throw new ApplicationException("Failed to create registry key");

			registryKey.SetValue("DefaultUsername", username, RegistryValueKind.String);
			registryKey.SetValue("DefaultPassword", password, RegistryValueKind.String);
			registryKey.SetValue("AutoAdminLogon", "1", RegistryValueKind.String);
		}
	}
}
#endif
