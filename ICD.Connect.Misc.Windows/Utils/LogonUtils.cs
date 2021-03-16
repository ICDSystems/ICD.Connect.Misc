#if !SIMPLSHARP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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
			username = username ?? string.Empty;
			password = password ?? string.Empty;

			const string key = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key, true) ?? Registry.LocalMachine.CreateSubKey(key);
			if (registryKey == null)
				throw new ApplicationException("Failed to create registry key");

			registryKey.SetValue("DefaultUsername", username, RegistryValueKind.String);
			registryKey.SetValue("DefaultPassword", password, RegistryValueKind.String);
			registryKey.SetValue("AutoAdminLogon", "1", RegistryValueKind.String);
		}

		/// <summary>
		/// Gets the username for the user that is currently logged in.
		/// </summary>
		/// <returns></returns>
		public static string GetCurrentUsername()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
			string domainAndUsername = (string)searcher.Get().Cast<ManagementBaseObject>().First()["UserName"];
			return domainAndUsername.Split('\\').LastOrDefault();
		}

		/// <summary>
		/// Gets the usernames recognized by the system.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<string> GetUsernames()
		{
			SelectQuery query = new SelectQuery("Win32_UserAccount");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
			return searcher.Get().Cast<ManagementObject>().Select(m => (string)m["Name"]);
		}
	}
}
#endif
