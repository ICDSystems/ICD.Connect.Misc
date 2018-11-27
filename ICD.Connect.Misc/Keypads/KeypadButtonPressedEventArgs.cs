using System;

namespace ICD.Connect.Misc.Keypads
{
	public sealed class KeypadButtonPressedEventArgs : EventArgs
	{
		private readonly uint m_ButtonId;
		private readonly eButtonState m_ButtonState;

		/// <summary>
		/// Gets the button ID.
		/// </summary>
		public uint ButtonId { get { return m_ButtonId; } }

		/// <summary>
		/// Gets the button state.
		/// </summary>
		public eButtonState ButtonState { get { return m_ButtonState; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="buttonId"></param>
		/// <param name="buttonState"></param>
		public KeypadButtonPressedEventArgs(uint buttonId, eButtonState buttonState)
		{
			m_ButtonId = buttonId;
			m_ButtonState = buttonState;
		}
	}
}