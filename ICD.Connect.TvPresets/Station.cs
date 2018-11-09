using System;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.TvPresets
{
	/// <summary>
	/// Represents a TV Station on a TV Tuner.
	/// </summary>
	public struct Station : IEquatable<Station>
	{
		private readonly string m_Channel;
		private readonly string m_Name;
		private readonly string m_Image;
		private readonly string m_Url;

		#region Properties

		/// <summary>
		/// Gets the channel number.
		/// </summary>
		public string Channel { get { return m_Channel ?? ""; } }

		/// <summary>
		/// Gets the label text to show in the UI.
		/// </summary>
		public string Name { get { return m_Name ?? ""; } }

		/// <summary>
		/// Gets the image serial.
		/// </summary>
		public string Image { get { return m_Image ?? ""; } }

		/// <summary>
		/// Gets the image url.
		/// </summary>
		public string Url { get { return m_Url ?? ""; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="name"></param>
		/// <param name="channel"></param>
		/// <param name="url"></param>
		public Station(string image, string name, string channel, string url)
		{
			m_Image = image;
			m_Name = name;
			m_Channel = channel;
			m_Url = url;
		}

		/// <summary>
		/// Instantiates a station from xml.
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static Station FromXml(string baseUrl, string xml)
		{
			string image = XmlUtils.GetAttribute(xml, "Image");
			string name = XmlUtils.GetAttribute(xml, "Name");
			string channel = XmlUtils.GetAttribute(xml, "Channel");
			string path = (baseUrl ?? string.Empty) + XmlUtils.ReadElementContent(xml);

			return new Station(image, name, channel, path);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Implementing default equality.
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public static bool operator ==(Station s1, Station s2)
		{
			return s1.Equals(s2);
		}

		/// <summary>
		/// Implementing default inequality.
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public static bool operator !=(Station s1, Station s2)
		{
			return !s1.Equals(s2);
		}

		/// <summary>
		/// Returns true if this instance is equal to the given object.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool Equals(object other)
		{
			return other is Station && Equals((Station)other);
		}

		public bool Equals(Station other)
		{
			return m_Channel == other.m_Channel &&
			       m_Name == other.m_Name &&
			       m_Image == other.m_Image &&
			       m_Url == other.m_Url;
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
				hash = hash * 23 + (m_Name == null ? 0 : m_Name.GetHashCode());
				hash = hash * 23 + (m_Image == null ? 0 : m_Image.GetHashCode());
				hash = hash * 23 + (m_Channel == null ? 0 : m_Channel.GetHashCode());
				hash = hash * 23 + (m_Url == null ? 0 : m_Url.GetHashCode());
				return hash;
			}
		}

		#endregion
	}
}
