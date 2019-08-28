using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class VolumeComponent : AbstractVibeComponent
	{
		public event EventHandler<VolumeChangedEventArgs> OnVolumeChanged;

		private const string COMMAND = "volume";
		private const string PARAM_GET_VOLUME = "-l";
		private const string PARAM_SET_VOLUME = "-s {0}";
		private const string PARAM_RAMP_UP = "up";
		private const string PARAM_RAMP_DOWN = "down";

		private int m_Volume;

		public int Volume 
		{ 
			get { return m_Volume; }
			private set
			{
				if (value == m_Volume)
					return;

				m_Volume = value;
				OnVolumeChanged.Raise(this, new VolumeChangedEventArgs(m_Volume));
			}
		}

		public VolumeComponent(VibeBoard parent) : base(parent)
		{
		}

		public void GetCurrentVolume()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_GET_VOLUME));
		}

		public void SetVolume(int volume)
		{
			if (volume < 0 || volume > 100)
				throw new ArgumentException(string.Format("Invalid argument: {0}. Volume must be between 0 and 100.", volume));

			string param = string.Format(PARAM_SET_VOLUME, volume);
			Parent.SendCommand(new VibeCommand(COMMAND, param));
		}

		public void VolumeUp()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_RAMP_UP));
		}

		public void VolumeDown()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_RAMP_DOWN));
		}

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Volume", Volume);
		}

		#endregion
	}

	public sealed class VolumeChangedEventArgs : GenericEventArgs<int>
	{
		public VolumeChangedEventArgs(int data) : base(data)
		{
		}
	}
}
