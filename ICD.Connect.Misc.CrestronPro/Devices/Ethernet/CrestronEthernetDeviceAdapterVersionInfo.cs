using System;
using System.Text.RegularExpressions;
#if SIMPLSHARP
using Crestron.SimplSharp;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Ethernet
{
	public struct CrestronEthernetDeviceAdapterVersionInfo : IEquatable<CrestronEthernetDeviceAdapterVersionInfo>
	{
		#region Fields

		private readonly string m_Model;
		private readonly string m_FirmwareVersion;
		private readonly string m_FirmwareDate;
		private readonly string m_Tsid;
		private readonly string m_SerialNumber;

		#endregion

		#region Properties

		public string Model { get { return m_Model; } }

		public string FirmwareVersion { get { return m_FirmwareVersion; } }

		public string FirmwareDate { get { return m_FirmwareDate; } }

		public string Tsid { get { return m_Tsid; } }

		public string SerialNumber { get { return m_SerialNumber; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="firmwareVersion"></param>
		/// <param name="firmwareDate"></param>
		/// <param name="tsid"></param>
		public CrestronEthernetDeviceAdapterVersionInfo(string model, string firmwareVersion,
		                                                string firmwareDate, string tsid)
		{
			m_Model = model;
			m_FirmwareVersion = firmwareVersion;
			m_FirmwareDate = firmwareDate;
			m_Tsid = tsid;
#if SIMPLSHARP
			// Attempt to convert TSID to a serial number.
			try
			{
				m_SerialNumber = CrestronEnvironment.ConvertTSIDToSerialNumber(tsid);
			}
			catch (Exception e)
			{
				// TODO log errors
				m_SerialNumber = null;
			}
#endif
#if STANDARD
			m_SerialNumber = null;
#endif
		}

		/// <summary>
		/// Parses version information from a Regex Match.
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public static CrestronEthernetDeviceAdapterVersionInfo Parse(Match match)
		{
			if (!match.Success)
				throw new InvalidOperationException("Unable to find a matching pattern in Version data");

			string model = match.Groups["model"].Value;
			string version = match.Groups["version"].Value;
			string date = match.Groups["date"].Value;
			string tsid = match.Groups["tsid"].Value;

			return new CrestronEthernetDeviceAdapterVersionInfo(model, version, date, tsid);
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
			(CrestronEthernetDeviceAdapterVersionInfo c1, CrestronEthernetDeviceAdapterVersionInfo c2)
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
			(CrestronEthernetDeviceAdapterVersionInfo c1, CrestronEthernetDeviceAdapterVersionInfo c2)
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
			return obj is CrestronEthernetDeviceAdapterVersionInfo && Equals((CrestronEthernetDeviceAdapterVersionInfo)obj);
		}

		public bool Equals(CrestronEthernetDeviceAdapterVersionInfo other)
		{
			return m_Model == other.m_Model &&
			       m_FirmwareVersion == other.m_FirmwareVersion &&
			       m_FirmwareDate == other.m_FirmwareDate &&
			       m_Tsid == other.m_Tsid;
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
				hash = hash * 23 + (m_Model == null ? 0 : m_Model.GetHashCode());
				hash = hash * 23 + (m_FirmwareVersion == null ? 0 : m_FirmwareVersion.GetHashCode());
				hash = hash * 23 + (m_FirmwareDate == null ? 0 : m_FirmwareDate.GetHashCode());
				hash = hash * 23 + (m_Tsid == null ? 0 : m_Tsid.GetHashCode());
				return hash;
			}
		}

		#endregion
	}
}