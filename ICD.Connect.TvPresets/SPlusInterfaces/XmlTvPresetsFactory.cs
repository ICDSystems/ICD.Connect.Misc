using System;
using System.Linq;
using System.Text;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
#if SIMPLSHARP
using Crestron.SimplSharp;
#endif
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.IO;

namespace ICD.Connect.TvPresets.SPlusInterfaces
{
	[PublicAPI]
	public static class XmlTvPresetsFactory
	{
		private const string SUBDIR = "TV Presets";
		private const string EXT = ".xml";
		private const int LOAD_EVENT_TIMEOUT = 1 * 1000;


#if SIMPLSHARP
		public delegate void StationParsedDelegate(ushort index, SimplSharpString channel, SimplSharpString name,
		                                           SimplSharpString image, SimplSharpString url);
#else
		public delegate void StationParsedDelegate(ushort index, string channel, string name, string image, string url);
#endif

// ReSharper disable once InconsistentNaming
		private enum PresetLoadState
		{
			NotLoaded,
			Loading,
			Loaded,
			LoadError
		}


		#region Fields      

		private static XmlTvPresets s_Presets = new XmlTvPresets();

		private static PresetLoadState s_PresetLoadState = PresetLoadState.NotLoaded;

		private static readonly SafeCriticalSection s_PresetsLoadCriticalSection = new SafeCriticalSection();

#if SIMPLSHARP
        private static readonly CEvent s_PresetsLoadedEvent = new CEvent(false, false);
#endif

#endregion

		/// <summary>
		/// Gets the directory where xml tv presets documents are located.
		/// </summary>
		[PublicAPI]
		public static string PresetsPath { get { return PathUtils.Join(PathUtils.NvramPath, SUBDIR); } }

		public static XmlTvPresets Presets
		{
			get { return GetOrLoadPresets(); }
		}

#region Events

		public static event EventHandler OnPresetsLoaded;


#endregion


#region Methods


		private static XmlTvPresets GetOrLoadPresets()
		{
			//Get the current state, if not loaded, set to loading
			s_PresetsLoadCriticalSection.Enter();
			PresetLoadState loadState = s_PresetLoadState;
			if (s_PresetLoadState == PresetLoadState.NotLoaded)
				s_PresetLoadState = PresetLoadState.Loading;
			s_PresetsLoadCriticalSection.Leave();

			// Return presets/null, wait for loading, or load now
			switch (loadState)
			{
				case PresetLoadState.Loaded:
					return s_Presets;
				case PresetLoadState.Loading:
#if SIMPLSHARP
                    s_PresetsLoadedEvent.Wait();
#endif
					return s_Presets;
				case PresetLoadState.LoadError:
					return s_Presets;
				case PresetLoadState.NotLoaded:
					LoadDefault();
					return s_Presets;
			}

			return null;
		}

		private static void PresetsLoaded()
		{
			EventHandler handler = OnPresetsLoaded;
			if (handler != null)
				handler(null, EventArgs.Empty);
		}


		public static void LoadDefault()
		{
			string basePath = PresetsPath;
			string xmlPath = IcdDirectory.GetFiles(basePath)
									  .Where(f =>
											 string.Equals(IcdPath.GetExtension(f), EXT, StringComparison.CurrentCultureIgnoreCase))
									  .Select(f => PathUtils.Join(basePath, f))
									  .FirstOrDefault();
			LoadPresets(xmlPath);
			
		}

		/// <summary>
		/// Finds the first xml document in the tv presets directory and attempts to load it.
		/// </summary>
		[PublicAPI]
		public static void LoadPresets(string xmlPath)
		{
#if SIMPLSHARP
			s_PresetsLoadedEvent.Reset();
#endif

			s_PresetsLoadCriticalSection.Enter();
			s_PresetLoadState = PresetLoadState.Loading;
			s_PresetsLoadCriticalSection.Leave();

			bool error = false;

			try
			{
				string xml = IcdFile.ReadToEnd(xmlPath, Encoding.UTF8);
				s_Presets = XmlTvPresets.FromXml(xml);
			}
			catch (Exception e)
			{
				ServiceProvider.TryGetService<ILoggerService>().AddEntry(eSeverity.Error, "Failed to parse TvPresets XML {0} - {1}", xmlPath, e.Message);
				s_Presets = new XmlTvPresets();
				error = true;
			}

			// Update LoadState
			s_PresetsLoadCriticalSection.Enter();
			s_PresetLoadState = error ? PresetLoadState.LoadError : PresetLoadState.Loaded;
			s_PresetsLoadCriticalSection.Leave();

#if SIMPLSHARP
			s_PresetsLoadedEvent.Set();
#endif

            PresetsLoaded();
		}

#endregion
	}
}
