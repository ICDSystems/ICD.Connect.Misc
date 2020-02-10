using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class PackageComponent : AbstractVibeComponent
	{
		public event EventHandler OnPackagesUpdated;

		private const string COMMAND = "packages";
		private const string PARAM_LIST = "list";

		private readonly List<PackageData> m_Packages;

		#region Properties

		public IEnumerable<PackageData> Packages
		{
			get { return m_Packages.AsReadOnly(); }
		}

		#endregion

		public PackageComponent(VibeBoard parent)
			: base(parent)
		{
			m_Packages = new List<PackageData>();
			Subscribe(parent);
		}

		protected override void Dispose(bool disposing)
		{
			Unsubscribe(Parent);

			base.Dispose(disposing);
		}

		#region API Methods

		public void ListPackages()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_LIST));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called to initialize the component.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			ListPackages();
		}

		#endregion

		#region Parent Callbacks

		/// <summary>
		/// Subscribes to the vibe events.
		/// </summary>
		/// <param name="vibe"></param>
		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.RegisterResponseCallback<ListPackageResponse>(ListPackageResponseCallback);
		}

		/// <summary>
		/// Unsubscribes from the vibe events.
		/// </summary>
		/// <param name="vibe"></param>
		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);

			if (vibe == null)
				return;

			vibe.ResponseHandler.UnregisterResponseCallback<ListPackageResponse>(ListPackageResponseCallback);
		}

		private void ListPackageResponseCallback(ListPackageResponse response)
		{
			if (response.Error != null)
			{
				Log(eSeverity.Error, "Failed to list packages - {0}", response.Error.Message);
				return;
			}

			m_Packages.Clear();
			m_Packages.AddRange(response.Value);

			Log(eSeverity.Informational, "Package list updated");
			OnPackagesUpdated.Raise(this);
		}

		#endregion

		#region Console

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (var command in base.GetConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("ListPackages", "Gets the list of installed packages", () => ListPackages());
		}

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Package Count", m_Packages.Count);
		}

		#endregion
	}
}
