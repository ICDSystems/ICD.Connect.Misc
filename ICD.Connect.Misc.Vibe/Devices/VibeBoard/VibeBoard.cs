using ICD.Common.Utils.EventArguments;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Displays.EventArguments;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components;
using ICD.Connect.Misc.Vibe.Settings;
using ICD.Connect.Protocol.EventArguments;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using ICD.Common.Utils;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard
{
	public class VibeBoard : AbstractDisplayWithAudio<VibeBoardSettings>
	{
		public event EventHandler<BoolEventArgs> OnConnectedStateChanged;
		public event EventHandler<BoolEventArgs> OnInitializedChanged;

		private const string REGEX_RESPONSE =
			"\"type\"\\s*:\\s*\"(?'type'[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*)\"(?:\\s*,.*\"resultId\"\\s*:\\s*\"(?'resultId'[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*)\")?";

		private readonly VibeComponentFactory m_ComponentFactory;

		public VibeComponentFactory Components { get { return m_ComponentFactory; } }

		public VibeBoard()
		{
			m_ComponentFactory = new VibeComponentFactory(this);
		}

		public override void PowerOn()
		{
			Components.GetComponent<ScreenComponent>().SetScreenState(true);
		}

		public override void PowerOff()
		{
			Components.GetComponent<ScreenComponent>().SetScreenState(false);
		}

		public override void SetActiveInput(int address)
		{
			// only one input...
		}

		public override void SetScalingMode(eScalingMode mode)
		{
			// not supported
		}

		protected override void SerialQueueOnSerialTransmission(object sender, SerialTransmissionEventArgs args)
		{
		}

		protected override void SerialQueueOnSerialResponse(object sender, SerialResponseEventArgs args)
		{
			
		}

		protected override void SerialQueueOnTimeout(object sender, SerialDataEventArgs args)
		{
			
		}

		public override void VolumeUpIncrement()
		{
			Components.GetComponent<VolumeComponent>().VolumeUp();
		}

		public override void VolumeDownIncrement()
		{
			Components.GetComponent<VolumeComponent>().VolumeDown();
		}

		protected override void VolumeSetRawFinal(float raw)
		{
			int volume = (int)MathUtils.MapRange(0.0f, 1.0f, 0, 100, raw);
			Components.GetComponent<VolumeComponent>().SetVolume(volume);
		}

		public override void MuteOn()
		{
			Components.GetComponent<MuteComponent>().MuteOn();
		}

		public override void MuteOff()
		{
			Components.GetComponent<MuteComponent>().MuteOff();
		}
	}
}
