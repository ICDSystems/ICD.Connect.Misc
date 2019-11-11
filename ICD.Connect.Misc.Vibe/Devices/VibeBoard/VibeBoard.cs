using ICD.Common.Utils.EventArguments;
using ICD.Connect.Displays.EventArguments;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components;
using ICD.Connect.Misc.Vibe.Settings;
using ICD.Connect.Protocol.EventArguments;
using System;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls;
using ICD.Connect.Panels.Server;
using ICD.Connect.Protocol.Data;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Mock.Destination;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard
{
	public class VibeBoard : AbstractPanelServerDevice<VibeBoardSettings>
	{
		public event EventHandler<BoolEventArgs> OnConnectedStateChanged;
		public event EventHandler<BoolEventArgs> OnInitializedChanged;


		private readonly VibeComponentFactory m_ComponentFactory;

		public VibeComponentFactory Components { get { return m_ComponentFactory; } }

		public VibeBoard()
		{
			m_ComponentFactory = new VibeComponentFactory(this);

			MockRouteDestinationControl routingControl = new MockRouteDestinationControl(this, 0);
			routingControl.SetInputs(new[] {new ConnectorInfo(1, eConnectionType.Video | eConnectionType.Audio)});

			Controls.Add(routingControl);
			Controls.Add(new VibeBoardVolumeControl(this, Controls.Count));
			Controls.Add(new VibeBoardPowerControl(this, Controls.Count));
		}

		public void SendCommand(VibeCommand command)
		{
			
		}

		protected override bool GetIsOnlineStatus()
		{
			// TODO: online status
			return true;
		}
	}
}
