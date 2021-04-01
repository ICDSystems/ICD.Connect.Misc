using System.Collections.Generic;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
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
			Parent.Logger.Log(eSeverity.Debug, "Pressing key {0}", key);
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
				Parent.Logger.Log(eSeverity.Error, "Failed to press key - {0}", response.Error.Message);
				return;
			}

			Parent.Logger.Log(eSeverity.Debug, "Key successfully pressed");
		}

		#endregion

		#region Console

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (var command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<eVibeKey>("Press", "Press <Back, Home, Task, Up, Down, Left, Right>",
				key => KeyPress(key));
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
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
