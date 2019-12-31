using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class PackageComponent : AbstractVibeComponent
	{
		private const string COMMAND = "packages";
		private const string PARAM_LIST = "list";

		private readonly List<PackageData> m_Packages;

		public IEnumerable<PackageData> Packages
		{
			get { return m_Packages.ToList(); }
		}

		public PackageComponent(VibeBoard parent) : base(parent)
		{
			m_Packages = new List<PackageData>();
		}

		public void ListPackages()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_LIST));
		}

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			vibe.ResponseHandler.RegisterResponseCallback<ListPackageResponse>(ListPackageResponseCallback);
		}

		private void ListPackageResponseCallback(ListPackageResponse response)
		{
			if (response.Value == null || response.Error != null)
				return;

			m_Packages.Clear();
			m_Packages.AddRange(response.Value);
		}
	}
}
