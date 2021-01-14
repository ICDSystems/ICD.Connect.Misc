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

		#endregion

		#region Regex

		/// <summary>
		/// Regex for matching the result of the 'ipconfig' command on Crestron ethernet devices.
		/// </summary>
		private const string IP_CONFIG_REGEX =
			@"(?:Link Status.+:\s+(?'LinkStatus'\S+))\s*(?:DHCP\s+.+:\s+(?'DHCP'\S+))\s*(?:MAC Address\s+.+:\s+(?'MACAddress'\S+))\s*(?:IP Address\s+.+:\s+(?'IPV4'\S+))\s*(?:Subnet Mask\s+.+:\s+(?'SubnetMask'\S+))\s*(?:IPV6 Address\s+.+:\s+(?'IPV6'\S+))\s*(?:Default Gateway\s+.+:\s+(?'DefaultGateway'\S+))\s*(?:DNS Servers\s+.+:\s+(?'DNS'\S+))";

		/// <summary>
		/// Regex for matching the result of the 'ver' command on Crestron ethernet devices.
		/// </summary>
		private const string VER_REGEX =
			@"Console*\s*(?'prompt'\S+>)?(?'model'\S+)\s+((?'type'\S+)\s+)?((?'lang'\S+)\s+)?\[v(?'version'\d+(\.\d+)+)\s+\((?'date'[^)]+)\),\s+#(?'tsid'[A-F0-9]+)\]\s*?(@E-(?'mac'[a-z0-9]+))?";

		#endregion

		#region Cache

		private static readonly SafeCriticalSection s_CacheSection;

		private static readonly
			WeakKeyDictionary<ICrestronEthernetDeviceAdapter, KeyValuePair<CrestronEthernetDeviceAdapterNetworkInfo, DateTime>>
			s_IpConfigCache;

		private static readonly
			WeakKeyDictionary<ICrestronEthernetDeviceAdapter, KeyValuePair<CrestronEthernetDeviceAdapterVersionInfo, DateTime>>
			s_VersionCache;

		#endregion

		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static CrestronEthernetDeviceUtils()
		{
			s_CacheSection = new SafeCriticalSection();
			s_IpConfigCache =
				new WeakKeyDictionary
					<ICrestronEthernetDeviceAdapter, KeyValuePair<CrestronEthernetDeviceAdapterNetworkInfo, DateTime>>();
			s_VersionCache =
				new WeakKeyDictionary
					<ICrestronEthernetDeviceAdapter, KeyValuePair<CrestronEthernetDeviceAdapterVersionInfo, DateTime>>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Connects to the adapter over SSH and returns network info about the adapter.
		/// </summary>
		/// <param name="adapter"></param>
		/// <returns></returns>
		public static CrestronEthernetDeviceAdapterNetworkInfo? GetNetworkInfo(ICrestronEthernetDeviceAdapter adapter)
		{
			s_CacheSection.Enter();
			try
			{
				// First see if the value was recently added to the cache.
				if (s_IpConfigCache.ContainsKey(adapter))
				{
					KeyValuePair<CrestronEthernetDeviceAdapterNetworkInfo, DateTime> kvp;
					s_IpConfigCache.TryGetValue(adapter, out kvp);

					if (kvp.Value + TimeSpan.FromMinutes(5) < IcdEnvironment.GetUtcTime())
						return kvp.Key;

					// Remove this adapter's entry since it is about to be updated.
					s_IpConfigCache.Remove(adapter);
				}

				// Otherwise get the value over SSH.
				bool timedOut;
				var networkInfo = RequestSsh(adapter, IP_CONFIG_COMMAND, TimeSpan.FromSeconds(5), out timedOut,
				                             new GenericExpectAction
					                             <CrestronEthernetDeviceAdapterNetworkInfo>
												 (new Regex(IP_CONFIG_REGEX),
					                              CrestronEthernetDeviceAdapterNetworkInfo.Parse));

				// If we got a response add it to the cache and return the value.
				if (!timedOut)
				{
					s_IpConfigCache.Add(adapter,
					                    new KeyValuePair<CrestronEthernetDeviceAdapterNetworkInfo, DateTime>(networkInfo,
					                                                                                         IcdEnvironment.GetUtcTime()));
					return networkInfo;
				}

				adapter.Logger.Log(eSeverity.Error, "Request for network information over SSH timed out");
				return null;
			}
			catch (Exception e)
			{
				adapter.Logger.Log(eSeverity.Error, "Error requesting network information over SSH - {0}\n{1}", e.Message, e.StackTrace);
				return null;
			}
			finally
			{
				s_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Connects to the adapter over SSH and returns version info about the adapter.
		/// </summary>
		/// <param name="adapter"></param>
		/// <returns></returns>
		public static CrestronEthernetDeviceAdapterVersionInfo? GetVersionInfo(ICrestronEthernetDeviceAdapter adapter)
		{
			s_CacheSection.Enter();
			try
			{
				// First see if the value was recently added to the cache.
				if (s_VersionCache.ContainsKey(adapter))
				{
					KeyValuePair<CrestronEthernetDeviceAdapterVersionInfo, DateTime> kvp;
					s_VersionCache.TryGetValue(adapter, out kvp);

					if (kvp.Value + TimeSpan.FromMinutes(5) < IcdEnvironment.GetUtcTime())
						return kvp.Key;

					// Remove this adapter's entry since it is about to be updated.
					s_VersionCache.Remove(adapter);
				}

				// Otherwise get the value over SSH.
				bool timedOut;
				var versionInfo = RequestSsh(adapter, VER_COMMAND, TimeSpan.FromSeconds(5), out timedOut,
				                             new GenericExpectAction
					                             <CrestronEthernetDeviceAdapterVersionInfo>
												 (new Regex(VER_REGEX),
					                              CrestronEthernetDeviceAdapterVersionInfo.Parse));
				if (!timedOut)
				{
					s_VersionCache.Add(adapter,
					                   new KeyValuePair<CrestronEthernetDeviceAdapterVersionInfo, DateTime>(versionInfo,
					                                                                                        IcdEnvironment.GetUtcTime()));
					return versionInfo;
				}

				adapter.Logger.Log(eSeverity.Error, "Request for version information over SSH timed out");
				return null;
			}
			catch (Exception e)
			{
				adapter.Logger.Log(eSeverity.Error, "Error requesting version information over SSH - {0}\n{1}", e.Message, e.StackTrace);
				return null;
			}
			finally
			{
				s_CacheSection.Leave();
			}
		}

		#endregion

		#region SSH

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