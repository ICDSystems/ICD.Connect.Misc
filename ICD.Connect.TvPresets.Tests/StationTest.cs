using NUnit.Framework;

namespace ICD.Connect.TvPresets.Tests
{
	[TestFixture]
	public sealed class StationTest
	{
		#region Properties

		[TestCase("test")]
		public void ChannelTest(string channel)
		{
			Station station = new Station(null, null, channel, null);

			Assert.AreEqual(channel, station.Channel);
		}

		[TestCase("test")]
		public void NameTest(string name)
		{
			Station station = new Station(null, name, null, null);

			Assert.AreEqual(name, station.Name);
		}

		[TestCase("test")]
		public void ImageTest(string image)
		{
			Station station = new Station(image, null, null, null);

			Assert.AreEqual(image, station.Image);
		}

		[TestCase("test")]
		public void UrlTest(string url)
		{
			Station station = new Station(null, null, null, url);

			Assert.AreEqual(url, station.Url);
		}

		#endregion

		#region Constructors

		[TestCase("http://10.40.3.75/TvPresets/Icons/")]
		public void FromXmlTest(string baseUrl)
		{
			const string xml = @"<Station Image=""ABC"" Name=""ABC"" Channel=""806"">TV_abc.png</Station>";

			Station station = Station.FromXml(baseUrl, xml);

			Assert.AreEqual("ABC", station.Image);
			Assert.AreEqual("ABC", station.Name);
			Assert.AreEqual("806", station.Channel);
			Assert.AreEqual("http://10.40.3.75/TvPresets/Icons/TV_abc.png", station.Url);
		}

		#endregion
	}
}
