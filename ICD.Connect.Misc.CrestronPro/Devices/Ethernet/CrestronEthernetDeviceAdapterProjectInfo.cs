using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Misc.CrestronPro.Devices.Ethernet
{
	public struct CrestronEthernetDeviceAdapterProjectInfo : IEquatable<CrestronEthernetDeviceAdapterProjectInfo>
	{
		#region Fields

		private readonly string m_Vtz;
		private readonly string m_Date;
		private readonly string m_Panel;
		private readonly bool m_Video;
		private readonly bool m_Rgb;
		private readonly bool m_Png;
		private readonly string m_Rackname;
		private readonly string m_MinCore3UiLevel;
		private readonly string m_ProjectPlatform;
		private readonly string m_Orientation;
		private readonly string m_VtProE;
		private readonly string m_Database;

		#endregion

		#region Properties

		[PublicAPI]
		public string Vtz { get { return m_Vtz; } }

		[PublicAPI]
		public string Date { get { return m_Date; } }

		[PublicAPI]
		public string Panel { get { return m_Panel; } }

		[PublicAPI]
		public bool Video { get { return m_Video; } }

		[PublicAPI]
		public bool Rgb { get { return m_Rgb; } }

		[PublicAPI]
		public bool Png { get { return m_Png; } }

		[PublicAPI]
		public string Rackname { get { return m_Rackname; } }

		[PublicAPI]
		public string MinCore3UiLevel { get { return m_MinCore3UiLevel; } }

		[PublicAPI]
		public string ProjectPlatform { get { return m_ProjectPlatform; } }

		[PublicAPI]
		public string Orientation { get { return m_Orientation; } }

		[PublicAPI]
		public string VtProE { get { return m_VtProE; } }

		[PublicAPI]
		public string Database { get { return m_Database; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="vtz"></param>
		/// <param name="date"></param>
		/// <param name="panel"></param>
		/// <param name="video"></param>
		/// <param name="rgb"></param>
		/// <param name="png"></param>
		/// <param name="rackname"></param>
		/// <param name="minCore3UiLevel"></param>
		/// <param name="projectPlatform"></param>
		/// <param name="orientation"></param>
		/// <param name="vtProE"></param>
		/// <param name="database"></param>
		public CrestronEthernetDeviceAdapterProjectInfo(string vtz, string date, string panel, bool video, bool rgb, bool png,
		                                                string rackname, string minCore3UiLevel, string projectPlatform,
		                                                string orientation, string vtProE, string database)
		{
			m_Vtz = vtz;
			m_Date = date;
			m_Panel = panel;
			m_Video = video;
			m_Rgb = rgb;
			m_Png = png;
			m_Rackname = rackname;
			m_MinCore3UiLevel = minCore3UiLevel;
			m_ProjectPlatform = projectPlatform;
			m_Orientation = orientation;
			m_VtProE = vtProE;
			m_Database = database;
		}

		public static CrestronEthernetDeviceAdapterProjectInfo Parse(Match match)
		{
			if (!match.Success)
				throw new InvalidOperationException("Unable to find a matching pattern in project info data");

			Dictionary<string, string> kvps =
				match.Groups["kvps"].Value
				                    .Split('\r', '\n')
				                    .Select(s => Regex.Match(s, @"^(?'key'[^=]*)=(?'value'.*)$"))
									.Where(m => m.Success)
				                    .ToDictionary(subMatch => subMatch.Groups["key"].Value,
				                                  subMatch => subMatch.Groups["value"].Value);

			string vtz = kvps.GetDefault("VTZ");
			string date = kvps.GetDefault("Date");
			string panel = kvps.GetDefault("Panel");
			bool video = bool.Parse(kvps.GetDefault("Video", "false"));
			bool rgb = bool.Parse(kvps.GetDefault("RGB", "false"));
			bool png = bool.Parse(kvps.GetDefault("PNG", "false"));
			string rackname = kvps.GetDefault("Rackname");
			string minCore3UiLevel = kvps.GetDefault("MinCore3UILevel");
			string projectPlatform = kvps.GetDefault("ProjectPlatform");
			string orientation = kvps.GetDefault("Orientation");
			string vtProE = kvps.GetDefault("VTpro-e");
			string database = kvps.GetDefault("Database");

			return new CrestronEthernetDeviceAdapterProjectInfo(vtz, date, panel, video, rgb, png, rackname, minCore3UiLevel,
			                                                    projectPlatform, orientation, vtProE, database);
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
			(CrestronEthernetDeviceAdapterProjectInfo c1, CrestronEthernetDeviceAdapterProjectInfo c2)
		{
			return c1.Equals(c2);
		}

		/// <summary>
		/// Implemnting default inequality.
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator !=
			(CrestronEthernetDeviceAdapterProjectInfo c1, CrestronEthernetDeviceAdapterProjectInfo c2)
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
			return obj is CrestronEthernetDeviceAdapterProjectInfo && Equals((CrestronEthernetDeviceAdapterProjectInfo)obj);
		}

		public bool Equals(CrestronEthernetDeviceAdapterProjectInfo other)
		{
			return m_Vtz == other.m_Vtz &&
			       m_Date == other.m_Date &&
			       m_Panel == other.m_Panel &&
			       m_Video == other.m_Video &&
			       m_Rgb == other.m_Rgb &&
			       m_Png == other.m_Png &&
			       m_Rackname == other.m_Rackname &&
			       m_MinCore3UiLevel == other.m_MinCore3UiLevel &&
			       m_Orientation == other.m_Orientation &&
			       m_VtProE == other.m_VtProE &&
			       m_Database == other.m_Database;
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
				hash = hash * 23 + (m_Vtz == null ? 0 : m_Vtz.GetHashCode());
				hash = hash * 23 + (m_Date == null ? 0 : m_Date.GetHashCode());
				hash = hash * 23 + (m_Panel == null ? 0 : m_Panel.GetHashCode());
				hash = hash * 23 + m_Video.GetHashCode();
				hash = hash * 23 + m_Rgb.GetHashCode();
				hash = hash * 23 + m_Png.GetHashCode();
				hash = hash * 23 + (m_Rackname == null ? 0 : m_Rackname.GetHashCode());
				hash = hash * 23 + (m_MinCore3UiLevel == null ? 0 : m_MinCore3UiLevel.GetHashCode());
				hash = hash * 23 + (m_ProjectPlatform == null ? 0 : m_ProjectPlatform.GetHashCode());
				hash = hash * 23 + (m_Orientation == null ? 0 : m_Orientation.GetHashCode());
				hash = hash * 23 + (m_VtProE == null ? 0 : m_VtProE.GetHashCode());
				hash = hash * 23 + (m_Database == null ? 0 : m_Database.GetHashCode());
				return hash;
			}
		}

		#endregion
	}
}