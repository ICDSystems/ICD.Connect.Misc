using System.Collections.Generic;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class OTAComponent : AbstractVibeComponent
	{
		private const string COMMAND = "ota";
		private const string PARAM_HOST = "host";

		public OTAComponent(VibeBoard parent) : base(parent)
		{
			Subscribe(parent);
		}

		protected override void Dispose(bool disposing)
		{
			Unsubscribe(Parent);

			base.Dispose(disposing);
		}

		#region API Methods

		public void UpdateOTAHosts(params string[] hosts)
		{
			string param = string.Format("{0} {1}", PARAM_HOST, string.Join(";", hosts));
			Parent.SendCommand(new VibeCommand(COMMAND, param));
		}

		#endregion

		#region Parent Callbacks

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.RegisterResponseCallback<OTAResponse>(OTAResponseCallback);
		}

		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.UnregisterResponseCallback<OTAResponse>(OTAResponseCallback);
		}

		private void OTAResponseCallback(OTAResponse response)
		{
			if (response.Error != null)
			{
				Parent.Logger.Log(eSeverity.Error, "Failed to update OTA host(s) - {0}", response.Error.Message);
			}

			Parent.Logger.Log(eSeverity.Informational, "OTA host(s) updated");
		}

		#endregion

		#region Console

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (var command in GetBaseConsoleCommands())
				yield return command;

			yield return new ParamsConsoleCommand("SetOTAHosts", "Sets the OTA servers",
				hosts => UpdateOTAHosts(hosts));
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
