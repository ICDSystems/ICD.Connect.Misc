using System;
using System.Text.RegularExpressions;
using ICD.Connect.Devices.Telemetry.DeviceInfo;

namespace ICD.Connect.Misc.CrestronPro.Devices.Ethernet
{
	public struct CrestronEthernetDeviceAdapterNetworkInfo : IEquatable<CrestronEthernetDeviceAdapterNetworkInfo>
	{
		#region Fields

		private readonly bool m_Dhcp;
		private readonly IcdPhysicalAddress m_MacAddress;
		private readonly string m_IpAddress;
		private readonly string m_SubnetMask;
		private readonly string m_DefaultGateway;
		private readonly string m_DnsServer;

		#endregion

		#region Properties

		public bool Dhcp { get { return m_Dhcp; } }

		public IcdPhysicalAddress MacAddress { get { return m_MacAddress; } }

		public string IpAddress { get { return m_IpAddress; } }

		public string SubnetMask { get { return m_SubnetMask; } }

		public string DefaultGateway { get { return m_DefaultGateway; } }

		public string DnsServer { get { return m_DnsServer; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dhcp"></param>
		/// <param name="macAddress"></param>
		/// <param name="ipAddress"></param>
		/// <param name="subnetMask"></param>
		/// <param name="defaultGateway"></param>
		/// <param name="dnsServer"></param>
		public CrestronEthernetDeviceAdapterNetworkInfo(bool dhcp, IcdPhysicalAddress macAddress, string ipAddress, string subnetMask,
		                                                string defaultGateway, string dnsServer)
		{
			m_Dhcp = dhcp;
			m_MacAddress = macAddress == null ? null : macAddress.Clone();
			m_IpAddress = ipAddress;
			m_SubnetMask = subnetMask;
			m_DefaultGateway = defaultGateway;
			m_DnsServer = dnsServer;
		}

		/// <summary>
		/// Parses network information from a Regex Match.
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public static CrestronEthernetDeviceAdapterNetworkInfo Parse(Match match)
		{
			if (!match.Success)
				throw new InvalidOperationException("Unable to find a matching pattern in IP Config data");

			bool dhcp = match.Groups["DHCP"].Value.Equals("ON", StringComparison.OrdinalIgnoreCase);
			string mac = match.Groups["MACAddress"].Value;
			string ip = match.Groups["IPV4"].Value;
			string mask = match.Groups["SubnetMask"].Value;
			string gateway = match.Groups["DefaultGateway"].Value;
			string dns = match.Groups["DNS"].Value;

			IcdPhysicalAddress macAddress;
			IcdPhysicalAddress.TryParse(mac, out macAddress);

			return new CrestronEthernetDeviceAdapterNetworkInfo(dhcp, macAddress, ip, mask, gateway, dns);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Implementing default equality.
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator ==
			(CrestronEthernetDeviceAdapterNetworkInfo c1, CrestronEthernetDeviceAdapterNetworkInfo c2)
		{
			return c1.Equals(c2);
		}

		/// <summary>
		/// Implementing default inequality.
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator !=
			(CrestronEthernetDeviceAdapterNetworkInfo c1, CrestronEthernetDeviceAdapterNetworkInfo c2)
		{
			return !c1.Equals(c2);
		}

		/// <summary>
		/// Returns true if this instance is equal to the given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return obj is CrestronEthernetDeviceAdapterNetworkInfo && Equals((CrestronEthernetDeviceAdapterNetworkInfo)obj);
		}

		public bool Equals(CrestronEthernetDeviceAdapterNetworkInfo other)
		{
			return m_Dhcp == other.m_Dhcp &&
			       m_MacAddress == other.m_MacAddress &&
			       m_IpAddress == other.m_IpAddress &&
			       m_SubnetMask == other.m_SubnetMask &&
			       m_DefaultGateway == other.m_DefaultGateway &&
			       m_DnsServer == other.m_DnsServer;
		}

		/// <summary>
		/// Gets the hashcode for this instance.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + (m_Dhcp.GetHashCode());
				hash = hash * 23 + (m_MacAddress == null ? 0 : m_MacAddress.GetHashCode());
				hash = hash * 23 + (m_IpAddress == null ? 0 : m_IpAddress.GetHashCode());
				hash = hash * 23 + (m_SubnetMask == null ? 0 : m_SubnetMask.GetHashCode());
				hash = hash * 23 + (m_DefaultGateway == null ? 0 : m_DefaultGateway.GetHashCode());
				hash = hash * 23 + (m_DnsServer == null ? 0 : m_DnsServer.GetHashCode());
				return hash;
			}
		}

		#endregion
	}
}