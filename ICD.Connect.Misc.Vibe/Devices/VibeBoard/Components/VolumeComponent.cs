using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

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
		private const string PARAM_SUBSCRIBE = "subscribe";

		private int m_Volume;

		#region Properties

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

		#endregion

		public VolumeComponent(VibeBoard parent)
			: base(parent)
		{
			Subscribe(parent);
		}

		protected override void Dispose(bool disposing)
		{
			Unsubscribe(Parent);

			base.Dispose(disposing);
		}

		#region Methods

		public void GetCurrentVolume()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_GET_VOLUME));
		}

		public void SetVolume(int volume)
		{
			if (volume < 0 || volume > 100)
				throw new ArgumentOutOfRangeException("volume", string.Format("Invalid argument: {0}. Volume must be between 0 and 100.", volume));

			string param = string.Format(PARAM_SET_VOLUME, volume);
			Parent.SendCommand(new VibeCommand(COMMAND, param));
		}

		public void SubscribeVolumeChanges()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_SUBSCRIBE));
		}

		public void VolumeUp()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_RAMP_UP));
		}

		public void VolumeDown()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_RAMP_DOWN));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called to initialize the component.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			GetCurrentVolume();
			SubscribeVolumeChanges();
		}

		#endregion

		#region Parent Callbacks

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			vibe.ResponseHandler.RegisterResponseCallback<VolumeResponse>(VolumeResponseCallback);
		}

		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);

			vibe.ResponseHandler.UnregisterResponseCallback<VolumeResponse>(VolumeResponseCallback);
		}

		private void VolumeResponseCallback(VolumeResponse response)
		{
			if (response.Error != null)
			{
				Log(eSeverity.Error, "Failed to get/set volume - {0}", response.Error.Message);
				return;
			}

			Volume = response.Value.Volume;
		}

		#endregion

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Volume", Volume);
		}

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (var command in base.GetConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("GetVolume", "Gets the current volume", () => GetCurrentVolume());
			yield return new GenericConsoleCommand<int>("SetVolume", "Sets the current volume", (v) => SetVolume(v));
			yield return new ConsoleCommand("VolumeUp", "Increments the volume", () => VolumeUp());
			yield return new ConsoleCommand("VolumeDown", "Decrements the volume", () => VolumeDown());
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
