﻿using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class KeyComponent : AbstractVibeComponent
	{
		private const string COMMAND = "key";

		public KeyComponent(VibeBoard parent)
			: base(parent)
		{
			Subscribe(parent);
		}

		protected override void Dispose(bool disposing)
		{
			Unsubscribe(Parent);

			base.Dispose(disposing);
		}

		#region API Methods

		public void KeyPress(eVibeKey key)
		{
			Log(eSeverity.Debug, "Pressing key {0}", key);
			Parent.SendCommand(new VibeCommand(COMMAND, key.ToString().ToLower()));
		}

		#endregion

		#region Parent Callbacks

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.RegisterResponseCallback<KeyInputResponse>(KeyInputCallback);
		}

		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.UnregisterResponseCallback<KeyInputResponse>(KeyInputCallback);
		}

		private void KeyInputCallback(KeyInputResponse response)
		{
			if (response.Error != null)
			{
				Log(eSeverity.Error, "Failed to press key - {0}", response.Error.Message);
				return;
			}

			Log(eSeverity.Debug, "Key successfully pressed");
		}

		#endregion
	}

	public enum eVibeKey
	{
		Back,
		Home,
		Up,
		Down,
		Left,
		Right,
		Task
	}
}
