using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Misc.CrestronPro.Devices.Ethernet;
#if SIMPLSHARP
using Crestron.SimplSharp;
using Crestron.SimplSharp.Ssh;
using Crestron.SimplSharpPro.CrestronThread;
using IAsyncResult = Crestron.SimplSharp.CrestronIO.IAsyncResult;
#endif
#if STANDARD
using System.Diagnostics;
using System.Threading;
using Renci.SshNet;
#endif

namespace ICD.Connect.Misc.CrestronPro.Utils
{
	public sealed class CrestronEthernetDeviceUtils
	{
		#region Commands

		private const string IP_CONFIG_COMMAND = "ipconfig";
		private const string VER_COMMAND = "ver";
		private const string PROJECT_INFO_COMMAND = "projectinfo";
		private const string APP_MODE_COMMAND = "appmode";


		#endregion

		#region Regex

		/// <summary>
		/// Regex for matching the result of the 'ipconfig' command on Crestron ethernet devices.
		/// </summary>
		private const string IP_CONFIG_REGEX =
			@"(?:Link Status.+:\s+(?'LinkStatus'\S+))\s*(?:DHCP\s+.+:\s+(?'DHCP'\S+))\s*(?:MAC Address(?:\(es\))*\s*.+:\s+(?'MACAddress'\S+))\s*(?:IP Address\s+.+:\s+(?'IPV4'\S+))\s*(?:Subnet Mask\s+.+:\s+(?'SubnetMask'\S+))\s*(?:(?:IPv6|IPV6) Address\s*.+:\s+(?'IPV6'\S+))\s*(?:Default Gateway\s+.+:\s+(?'DefaultGateway'\S+))\s*(?:DNS Servers\s*.+:\s+(?'DNS'\S+))";

		/// <summary>
		/// Regex for matching the result of the 'ver' command on Crestron ethernet devices.
		/// </summary>
		private const string VER_REGEX =
			@"Console*\s*(?'prompt'\S+>)?(?'model'\S+)\s+((?'type'\S+)\s+)?((?'lang'\S+)\s+)?\[v(?'version'\d+(\.\d+)+)\s+\((?'date'[^)]+)\),\s+#(?'tsid'[A-F0-9]+)\]\s*?(@E-(?'mac'[a-z0-9]+))?";

		/// <summary>
		/// Regex for matching the result of the 'projectinfo' command on Crestron ethernet devices.
		/// </summary>
		private const string PROJECT_INFO_REGEX =
			@"(?:\[BEGIN_INFO\])\s*(?:\[\S+\])?\s*(?'kvps'[\S\s]+)\s*(?:\[END_INFO\])";

		/// <summary>
		/// Regex for Matching the result of the 'appmode' command on Crestron ethernet devices.
		/// </summary>
		private const string APP_MODE_REGEX =
			@"(?:App mode:\s*(?'AppMode'.*))";

		#endregion

		#region Cache

		private static readonly SafeCriticalSection s_CacheSection;

		private static readonly
			WeakKeyDictionary<ICrestronEthernetDeviceAdapter, KeyValuePair<CrestronEthernetDeviceAdapterNetworkInfo, DateTime>>
			s_IpConfigCache;

		private static readonly
			WeakKeyDictionary<ICrestronEthernetDeviceAdapter, KeyValuePair<CrestronEthernetDeviceAdapterVersionInfo, DateTime>>
			s_VersionCache;

		private static readonly
			WeakKeyDictionary<ICrestronEthernetDeviceAdapter, KeyValuePair<CrestronEthernetDeviceAdapterProjectInfo, DateTime>>
			s_ProjectInfoCache;

		private static readonly
			WeakKeyDictionary<ICrestronEthernetDeviceAdapter, KeyValuePair<string, DateTime>>
			s_AppModeCache;

		#endregion

		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static CrestronEthernetDeviceUtils()
		{
			s_CacheSection = new SafeCriticalSection();

			// Initialize caches
			s_IpConfigCache =
				new WeakKeyDictionary
					<ICrestronEthernetDeviceAdapter, KeyValuePair<CrestronEthernetDeviceAdapterNetworkInfo, DateTime>>();
			s_VersionCache =
				new WeakKeyDictionary
					<ICrestronEthernetDeviceAdapter, KeyValuePair<CrestronEthernetDeviceAdapterVersionInfo, DateTime>>();
			s_ProjectInfoCache =
				new WeakKeyDictionary
					<ICrestronEthernetDeviceAdapter, KeyValuePair<CrestronEthernetDeviceAdapterProjectInfo, DateTime>>();
			s_AppModeCache =
				new WeakKeyDictionary
					<ICrestronEthernetDeviceAdapter, KeyValuePair<string, DateTime>>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Connects to the adapter over SSH and returns network info about the adapter.
		/// </summary>
		/// <param name="adapter"></param>
		/// <returns></returns>
		public static void UpdateNetworkInfo(ICrestronEthernetDeviceAdapter adapter, Action<CrestronEthernetDeviceAdapterNetworkInfo> updateAction)
		{
			// Already cached?
			CrestronEthernetDeviceAdapterNetworkInfo networkInfo;
			if (TryGetCachedValue(s_IpConfigCache, adapter, out networkInfo))
			{
				updateAction(networkInfo);
				return;
			}

			// Safely update the value.
			ThreadingUtils.SafeInvoke(() =>
			{
				if (TryRequestSsh(adapter, IP_CONFIG_COMMAND, IP_CONFIG_REGEX,
				                  CrestronEthernetDeviceAdapterNetworkInfo.Parse, out networkInfo) &&
				    InsertCachedValue(s_IpConfigCache, adapter, networkInfo))
				{
					updateAction(networkInfo);
				}
			});
		}

		/// <summary>
		/// Connects to the adapter over SSH and returns version info about the adapter.
		/// </summary>
		/// <param name="adapter"></param>
		/// <returns></returns>
		public static void UpdateVersionInfo(ICrestronEthernetDeviceAdapter adapter, Action<CrestronEthernetDeviceAdapterVersionInfo> updateAction)
		{
			// Already cached?
			CrestronEthernetDeviceAdapterVersionInfo versionInfo;
			if (TryGetCachedValue(s_VersionCache, adapter, out versionInfo))
			{
				updateAction(versionInfo);
				return;
			}

			// Safely update the value.
			ThreadingUtils.SafeInvoke(() =>
			{
				if (TryRequestSsh(adapter, VER_COMMAND, VER_REGEX,
				                  CrestronEthernetDeviceAdapterVersionInfo.Parse, out versionInfo) &&
				    InsertCachedValue(s_VersionCache, adapter, versionInfo))
				{
					updateAction(versionInfo);
				}
			});
		}

		/// <summary>
		/// Connects to the adapter over SSH and returns project info about the adapter.
		/// </summary>
		/// <param name="adapter"></param>
		/// <param name="updateAction"></param>
		/// <returns></returns>
		public static void UpdateProjectInfo(ICrestronEthernetDeviceAdapter adapter, Action<CrestronEthernetDeviceAdapterProjectInfo> updateAction)
		{
			// Already cached?
			CrestronEthernetDeviceAdapterProjectInfo projectInfo;
			if (TryGetCachedValue(s_ProjectInfoCache, adapter, out projectInfo))
			{
				updateAction(projectInfo);
				return;
			}

			// Safely update the value.
			ThreadingUtils.SafeInvoke(() =>
			{
				if (TryRequestSsh(adapter, PROJECT_INFO_COMMAND, PROJECT_INFO_REGEX,
				                  CrestronEthernetDeviceAdapterProjectInfo.Parse, out projectInfo) &&
				    InsertCachedValue(s_ProjectInfoCache, adapter, projectInfo))
				{
					updateAction(projectInfo);
				}
			});
		}

		/// <summary>
		/// Connects to the adapter over SSH and returns the app mode of the adapter.
		/// </summary>
		/// <param name="adapter"></param>
		/// <param name="updateAction"></param>
		/// <returns></returns>
		public static void UpdateAppMode(ICrestronEthernetDeviceAdapter adapter, Action<string> updateAction)
		{
			// Already cached?
			string appMode;
			if (TryGetCachedValue(s_AppModeCache, adapter, out appMode))
			{
				updateAction(appMode);
				return;
			}

			// Safely update the value.
			ThreadingUtils.SafeInvoke(() =>
			{
				if (TryRequestSsh(adapter, APP_MODE_COMMAND, APP_MODE_REGEX, m => m.Groups["AppMode"].Value.TrimEnd('\r', '\n'), out appMode) &&
				    InsertCachedValue(s_AppModeCache, adapter, appMode))
				{
					updateAction(appMode);
				}
			});
		}

		#endregion

		#region Cache Helpers

		/// <summary>
		/// Returns false if a newer value has already been cached.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="cache"></param>
		/// <param name="adapter"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool InsertCachedValue<TValue>(
			IDictionary<ICrestronEthernetDeviceAdapter, KeyValuePair<TValue, DateTime>> cache,
			ICrestronEthernetDeviceAdapter adapter, TValue value)
		{
			DateTime now = IcdEnvironment.GetUtcTime();

			s_CacheSection.Enter();

			try
			{
				// Already cached a newer result
				KeyValuePair<TValue, DateTime> kvp;
				if (cache.TryGetValue(adapter, out kvp) && kvp.Value > now)
					return false;

				cache[adapter] =
					new KeyValuePair<TValue, DateTime>(value, IcdEnvironment.GetUtcTime());
				return true;
			}
			finally
			{
				s_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Attempts to get the specified value from the cache.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="cache"></param>
		/// <param name="adapter"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		private static bool TryGetCachedValue<TValue>
			(IDictionary<ICrestronEthernetDeviceAdapter, KeyValuePair<TValue, DateTime>> cache,
			 ICrestronEthernetDeviceAdapter adapter,
			 out TValue output)
		{
			output = default(TValue);

			s_CacheSection.Enter();
			try
			{
				// First see if the value was recently added to the cache.
				KeyValuePair<TValue, DateTime> kvp;
				if (cache.TryGetValue(adapter, out kvp))
				{
					if (kvp.Value + TimeSpan.FromMinutes(5) > IcdEnvironment.GetUtcTime())
					{
						output = kvp.Key;
						return true;
					}
				}
			}
			finally
			{
				s_CacheSection.Leave();
			}

			return false;
		}

		#endregion

		#region SSH

		private static bool TryRequestSsh<TValue>(ICrestronEthernetDeviceAdapter adapter, string command,
		                                          string regex, Func<Match, TValue> parse, out TValue output)
		{
			output = default(TValue);

			bool timedOut;

			try
			{
				output = RequestSsh(adapter, command, TimeSpan.FromSeconds(5), out timedOut,
				                    new GenericExpectAction<TValue>(new Regex(regex), parse));
			}
			catch (Exception e)
			{
				adapter.Logger.Log(eSeverity.Error, "Error requesting {0} {1} over SSH - {2}\n{3}", adapter, typeof(TValue).Name,
				                   e.Message, e.StackTrace);
				return false;
			}

			// If we got a response add it to the cache and return the value.
			if (timedOut)
			{
				adapter.Logger.Log(eSeverity.Error, "Request for {0} {1} over SSH timed out", adapter, typeof(TValue).Name);
				return false;
			}

			return true;
		}

		private static TOutput RequestSsh<TOutput>(ICrestronEthernetDeviceAdapter adapter, string data, TimeSpan timeout,
		                                           out bool timedOut,
		                                           params GenericExpectAction<TOutput>[] expectActions)
		{
			bool handled = false;
			TOutput result = default(TOutput);

			ExpectAction[] expectActionWrappers =
				expectActions.Select(a => new ExpectAction(a.Regex, s =>
				                                                    {
					                                                    handled = true;
					                                                    result = a.Handle(s);
				                                                    }))
				             .ToArray();

			using (SshClient client = new SshClient(adapter.NetworkProperties.NetworkAddress,
			                                        adapter.NetworkProperties.NetworkPort ?? 22,
			                                        adapter.NetworkProperties.NetworkUsername,
			                                        adapter.NetworkProperties.NetworkPassword ?? string.Empty))
			{
				client.Connect();

				using (ShellStream shell =
					client.CreateShellStream(typeof(CrestronEthernetDeviceUtils).FullName,
					                         128, 100, 128, 100, 1024 * 1024))
				{
					// Start listening for results
					IAsyncResult asyncResult =
						shell.BeginExpect(timeout + TimeSpan.FromSeconds(1), // Let us handle the timeout first
						                  null,
						                  null,
						                  expectActionWrappers);

					// Send the command
					shell.WriteLine(data);

					Thread.Sleep(100);

					// Start timeout stopwatch
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();

					// Wait for success or timeout
					while (!asyncResult.IsCompleted && stopwatch.Elapsed < timeout)
						Thread.Sleep(100);

					stopwatch.Stop();

					shell.EndExpect(asyncResult);

					timedOut = !handled;
					return result;
				}
			}
		}

		#endregion

		private sealed class GenericExpectAction<T>
		{
			private readonly Regex m_Regex;
			private readonly Func<Match, T> m_Callback;

			public Regex Regex { get { return m_Regex; } }

			public T Handle(string line)
			{
				Match match = Regex.Match(line);
				return m_Callback(match);
			}

			public GenericExpectAction(Regex regex, Func<Match, T> callback)
			{
				m_Regex = regex;
				m_Callback = callback;
			}
		}
	}
}