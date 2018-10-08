using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.TvPresets
{
	/// <summary>
	/// Parses TV stations from xml.
	/// </summary>
	public sealed class XmlTvPresets : ITvPresets
	{
		private readonly List<Station> m_Stations;
		private readonly SafeCriticalSection m_StationsSection;

		#region Properties

		/// <summary>
		/// Gets the number of stations.
		/// </summary>
		public int Count { get { return m_Stations.Count; } }

		public bool IsReadOnly { get { return false; } }

		/// <summary>
		/// Gets the station at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Station this[int index] { get { return m_StationsSection.Execute(() => m_Stations[index]); } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public XmlTvPresets()
		{
			m_Stations = new List<Station>();
			m_StationsSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Instantiates a XmlTvPresets instance from an xml document.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static XmlTvPresets FromXml(string xml)
		{
			XmlTvPresets output = new XmlTvPresets();
			output.Parse(xml);
			return output;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Parses the presets xml document and adds the stations to the collection.
		/// </summary>
		/// <param name="xml"></param>
		[PublicAPI]
		public void Parse(string xml)
		{
			Clear();

			IEnumerable<Station> stations = ParseStations(xml);
			m_StationsSection.Execute(() => m_Stations.AddRange(stations));
		}

		/// <summary>
		/// Returns the stations from the given xml document.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<Station> ParseStations(string xml)
		{
			string baseUrl = XmlUtils.TryReadChildElementContentAsString(xml, "BaseUrl") ?? BuildDefaultBaseUrl();
			string stations = XmlUtils.GetChildElementAsString(xml, "Stations");

			return XmlUtils.GetChildElementsAsString(stations)
			               .Select(s => Station.FromXml(baseUrl, s));
		}

		private static string BuildDefaultBaseUrl()
		{
			return new IcdUriBuilder
			{
				Host = IcdEnvironment.NetworkAddresses.FirstOrDefault(),
				Path = "/TvPresets/Icons/",
			}.ToString();
		}

		public IEnumerator<Station> GetEnumerator()
		{
			// Copy the list and return THAT enumerator for thread safety.
			List<Station> output = m_StationsSection.Execute(() => m_Stations.ToList());
			return output.GetEnumerator();
		}

		public void Add(Station item)
		{
			m_StationsSection.Execute(() => m_Stations.Add(item));
		}

		public void Clear()
		{
			m_StationsSection.Execute(() => m_Stations.Clear());
		}

		public bool Contains(Station item)
		{
			return m_StationsSection.Execute(() => m_Stations.Contains(item));
		}

		public void CopyTo(Station[] array, int arrayIndex)
		{
			m_StationsSection.Execute(() => m_Stations.CopyTo(array, arrayIndex));
		}

		public bool Remove(Station item)
		{
			return m_StationsSection.Execute(() => m_Stations.Remove(item));
		}

		#endregion

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
