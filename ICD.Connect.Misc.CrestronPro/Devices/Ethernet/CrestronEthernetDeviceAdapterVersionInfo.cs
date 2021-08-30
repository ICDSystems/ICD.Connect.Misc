using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.TimeZoneInfo;
#if !NETSTANDARD
using Crestron.SimplSharp;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Ethernet
{
	public struct CrestronEthernetDeviceAdapterVersionInfo : IEquatable<CrestronEthernetDeviceAdapterVersionInfo>
	{
		#region Static Members

		private static readonly string[] s_DateFormats =
		{
			@"ddd MMM d HH:mm:ss yyyy",
			@"ddd MMM  d HH:mm:ss yyyy",
			@"MMMM d yyyy",
			@"MMM d yyyy"
		};

		private static readonly string[] s_DatePatterns =
		{
			@"(?:\S+\s+)(?:\S+\s+)(?:\d+\s+)(?:\d+:\d+:\d+\s+)(?'target'\S+)(?:\s+\d+)",
			@"(?:\S+\s+)(?:\d+\s+)(?:\d+)"
		};

		private static readonly Dictionary<string, string> s_TimeZoneAbbreviationsToNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{"ADT","Atlantic Daylight Time"},
			{"AKDT","Alaska Daylight Time"},
			{"AKST","Alaska Standard Time"},
			{"AST","Atlantic Standard Time"},
			{"AT","Atlantic Time"},
			{"CDT","Central Daylight Time"},
			{"CST","Central Standard Time"},
			{"CT","Central Time"},
			{"EDT","Eastern Daylight Time"},
			{"EGST","Eastern Greenland Summer Time"},
			{"EGT","East Greenland Time"},
			{"EST","Eastern Standard Time"},
			{"ET","Eastern Time"},
			{"GMT","Greenwich Mean Time"},
			{"HDT","Hawaii-Aleutian Daylight Time"},
			{"HST","Hawaii Standard Time"},
			{"MDT","Mountain Daylight Time"},
			{"MST","Mountain Standard Time"},
			{"MT","Mountain Time"},
			{"NDT","Newfoundland Daylight Time"},
			{"NST","Newfoundland Standard Time"},
			{"PDT","Pacific Daylight Time"},
			{"PST","Pacific Standard Time"},
			{"PT","Pacific Time"},
			{"UTC","Coordinated Universal Time"},
			{"WGST","Western Greenland Summer Time"},
			{"WGT","West Greenland Time"}
		};

		#endregion

		#region Fields

		private readonly string m_Model;
		private readonly string m_FirmwareVersion;
		private readonly DateTime? m_FirmwareDate;
		private readonly string m_Tsid;
		private readonly string m_SerialNumber;

		#endregion

		#region Properties

		public string Model { get { return m_Model; } }

		public string FirmwareVersion { get { return m_FirmwareVersion; } }

		public DateTime? FirmwareDate { get { return m_FirmwareDate; } }

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
		                                                DateTime? firmwareDate, string tsid)
		{
			m_Model = model;
			m_FirmwareVersion = firmwareVersion;
			m_FirmwareDate = firmwareDate;
			m_Tsid = tsid;
#if !NETSTANDARD
			// Attempt to convert TSID to a serial number.
			try
			{
				m_SerialNumber = CrestronEnvironment.ConvertTSIDToSerialNumber(tsid);
			}
			catch (Exception e)
			{
				IcdErrorLog.Error("Error converting TSID - \"{0}\" to serial number\n{1}\n{2}",
				                  tsid,
				                  e.Message,
				                  e.StackTrace);
				m_SerialNumber = null;
			}
#else
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
			string dateString = match.Groups["date"].Value;
			string tsid = match.Groups["tsid"].Value;

			// Attempt to parse DateTime from string.
			DateTime date;
			DateTime? nullable = TryParseCrestronFirmwareDateTimeString(dateString, out date) ? date : (DateTime?)null;

			return new CrestronEthernetDeviceAdapterVersionInfo(model, version, nullable, tsid);
		}

		private static bool TryParseCrestronFirmwareDateTimeString([NotNull] string dateTimeString, out DateTime result)
		{
			if (string.IsNullOrEmpty(dateTimeString))
				throw new FormatException("String must not be null or empty");

			result = default(DateTime);

			Match match = RegexUtils.MatchAny(dateTimeString, s_DatePatterns);
			if (!match.Success)
				return false;

			string timeZoneAbbr = match.Groups["target"].Value;
			string timeZoneId =
				timeZoneAbbr != null && s_TimeZoneAbbreviationsToNames.ContainsKey(timeZoneAbbr)
					? s_TimeZoneAbbreviationsToNames[timeZoneAbbr]
					: IcdEnvironment.GetLocalTimeZoneName();

			IcdTimeZoneInfo timezone;
			if (!IcdTimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out timezone))
				timezone = IcdTimeZoneInfo.FindSystemTimeZoneById(IcdEnvironment.GetLocalTimeZoneName());

			// Strip the timezone abbreviation
			string word = string.Format(" {0} ", timeZoneAbbr);
			string rawDateTimeString = dateTimeString.Replace(word, " ");

			try
			{
				DateTime parsed = DateTime.ParseExact(rawDateTimeString, s_DateFormats, null, DateTimeStyles.None);
				result = timezone.ConvertToUtc(parsed);
				return true;
			}
			catch (FormatException e)
			{
				IcdErrorLog.Error("Error parsing Crestron firmware date from string - {0}", e.Message);
				return false;
			}
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