using ICD.Connect.Devices.Controls;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls
{
	public sealed class VibeBoardPowerControl : AbstractPowerDeviceControl<VibeBoard>
	{
		private ScreenComponent m_ScreenComponent;

		public VibeBoardPowerControl(VibeBoard parent, int id) : base(parent, id)
		{
		}

		protected override void PowerOnFinal()
		{
			if (m_ScreenComponent == null)
				return;

			m_ScreenComponent.ScreenOn();
		}

		protected override void PowerOffFinal()
		{
			if (m_ScreenComponent == null)
				return;

			m_ScreenComponent.ScreenOff();
		}

		#region Parent

		protected override void Subscribe(VibeBoard parent)
		{
			base.Subscribe(parent);

			if (parent == null)
				return;

			m_ScreenComponent = parent.Components.GetComponent<ScreenComponent>();
		}

		protected override void Unsubscribe(VibeBoard parent)
		{
			base.Unsubscribe(parent);

			m_ScreenComponent = null;
		}

		#endregion
	}
}
