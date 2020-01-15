using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class PackageComponent : AbstractVibeComponent
	{
		private const string COMMAND = "packages";
		private const string PARAM_LIST = "list";

		public event EventHandler OnPackagesUpdated;

		private readonly List<PackageData> m_Packages;

		public IEnumerable<PackageData> Packages
		{
			get { return m_Packages.ToList(); }
		}

		public PackageComponent(VibeBoard parent)
			: base(parent)
		{
			m_Packages = new List<PackageData>();
		}

		/// <summary>
		/// Called to initialize the component.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			ListPackages();
		}

		#region Methods

		public void ListPackages()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_LIST));
		}

		#endregion

		#region Parent Callbacks

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			vibe.ResponseHandler.RegisterResponseCallback<ListPackageResponse>(ListPackageResponseCallback);
		}

		/// <summary>
		/// Unsubscribes from the vibe events.
		/// </summary>
		/// <param name="vibe"></param>
		protected override void Unsubscribe(VibeBoard vibe)
		{
			base.Unsubscribe(vibe);

			vibe.ResponseHandler.UnregisterResponseCallback<ListPackageResponse>(ListPackageResponseCallback);
		}

		private void ListPackageResponseCallback(ListPackageResponse response)
		{
			if (response.Value == null || response.Error != null)
				return;

			m_Packages.Clear();
			m_Packages.AddRange(response.Value);
			OnPackagesUpdated.Raise(this);
		}

		#endregion
	}
}
