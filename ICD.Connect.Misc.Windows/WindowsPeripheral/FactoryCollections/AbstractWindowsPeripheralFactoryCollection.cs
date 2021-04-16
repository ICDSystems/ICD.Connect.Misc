using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Connect.Misc.Windows.WindowsPeripheral.Factories;

namespace ICD.Connect.Misc.Windows.WindowsPeripheral.FactoryCollections
{
	public abstract class AbstractWindowsPeripheralFactoryCollection<TFactory> : IWindowsPeripheralFactoryCollection<TFactory>
		where TFactory : IWindowsPeripheralFactory
	{
		private readonly WindowsPeripheralComponent m_PeripheralComponent;

		private readonly IcdHashSet<TFactory> m_Factories;

		protected IEnumerable<TFactory> Factories { get { return m_Factories.ToArray(); } }

		protected WindowsPeripheralComponent PeripheralComponent { get { return m_PeripheralComponent; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="peripheralComponent"></param>
		protected AbstractWindowsPeripheralFactoryCollection(WindowsPeripheralComponent peripheralComponent)
		{
			m_PeripheralComponent = peripheralComponent;
			m_Factories = new IcdHashSet<TFactory>();
		}

		public abstract void AddFactoryFromXml(string xml);

		public virtual void AddFactory(TFactory factory)
		{
			m_Factories.Add(factory);
		}

		void IWindowsPeripheralFactoryCollection.AddFactory(IWindowsPeripheralFactory factory)
		{
			AddFactory((TFactory)factory);
		}

		/// <summary>
		/// When called, tells the peripheral factory collection to update its peripherals
		/// Typically called on StartSettings to add connected peripherals from the whitelist
		/// </summary>
		public abstract bool UpdatePeripherals();
	}
}
