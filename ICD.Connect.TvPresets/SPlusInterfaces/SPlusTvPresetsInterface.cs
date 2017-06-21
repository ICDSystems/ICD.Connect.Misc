using System;
using Crestron.SimplSharp;
using ICD.Common.Properties;

namespace ICD.Connect.TvPresets.SPlusInterfaces
{
	public sealed class SPlusTvPresetsInterface
	{

		#region Delegates

		public delegate void StationCountDelegate(ushort count);

		public delegate void StationParsedDelegate(ushort index, SimplSharpString channel, SimplSharpString name,
												   SimplSharpString image, SimplSharpString url);

		#endregion

		/// <summary>
		/// Called once before we start calling the StationParsedCallback to notify the number of parsed stations.
		/// </summary>
		[PublicAPI]
		public StationCountDelegate StationCountCallback { get; set; }

		/// <summary>
		/// Called once for each station that is parsed.
		/// </summary>
		[PublicAPI]
		public StationParsedDelegate StationParsedCallback { get; set; }

		public void UpdatePresetsToSimpl()
		{

			var presets = XmlTvPresetsFactory.Presets;
			if (StationCountCallback != null)
				StationCountCallback((ushort)presets.Count);

			if (StationParsedCallback == null)
				return;

			for (ushort index = 0; index < presets.Count; index++)
			{
				Station station = presets[index];
				StationParsedCallback(index, station.Channel, station.Name, station.Image, station.Url);
			}
		}


		public void InitializePresetsInterface()
		{
			UpdatePresetsToSimpl();
			XmlTvPresetsFactory.OnPresetsLoaded += XmlTvPresetsFactoryOnPresetsLoaded;
		}

		private void XmlTvPresetsFactoryOnPresetsLoaded(object sender, EventArgs eventArgs)
		{
			UpdatePresetsToSimpl();
		}
	}
}