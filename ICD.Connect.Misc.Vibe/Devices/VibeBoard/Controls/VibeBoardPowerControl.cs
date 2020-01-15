using ICD.Connect.Devices.Controls;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls
{
	public sealed class VibeBoardPowerControl : AbstractPowerDeviceControl<VibeBoard>
	{
		private ScreenComponent m_ScreenComponent;

		public VibeBoardPowerControl(VibeBoard parent, int id)
			: base(parent, id)
		{
		}

		protected override void PowerOnFinal()
		{
			if (m_ScreenComponent == null)
				return;

			switch (m_ScreenComponent.ScreenState)
			{
				case ePowerState.Unknown:
				case ePowerState.PowerOff:
				case ePowerState.Cooling:
					m_ScreenComponent.ScreenOn();
					break;
			}
		}

		protected override void PowerOffFinal()
		{
			if (m_ScreenComponent == null)
				return;

			switch (m_ScreenComponent.ScreenState)
			{
				case ePowerState.Unknown:
				case ePowerState.PowerOn:
				case ePowerState.Warming:
					m_ScreenComponent.ScreenOff();
					break;
			}
		}

		#region Parent

		protected override void Subscribe(VibeBoard parent)
		{
			base.Subscribe(parent);

			if (parent == null)
				return;
			
			m_ScreenComponent = parent.Components.GetComponent<ScreenComponent>();

			if (m_ScreenComponent != null)
				m_ScreenComponent.OnScreenStateChanged += ScreenComponentOnOnScreenStateChanged;
		}

		protected override void Unsubscribe(VibeBoard parent)
		{
			base.Unsubscribe(parent);

			if (m_ScreenComponent != null)
				m_ScreenComponent.OnScreenStateChanged -= ScreenComponentOnOnScreenStateChanged;

			m_ScreenComponent = null;
		}
		
		private void ScreenComponentOnOnScreenStateChanged(object sender, PowerStateEventArgs e)
		{
			PowerState = e.Data;
		}

		#endregion
	}
}
