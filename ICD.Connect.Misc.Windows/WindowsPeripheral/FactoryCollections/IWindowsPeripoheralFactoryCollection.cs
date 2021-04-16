using ICD.Connect.Misc.Windows.WindowsPeripheral.Factories;

namespace ICD.Connect.Misc.Windows.WindowsPeripheral.FactoryCollections
{
	interface IWindowsPeripheralFactoryCollection
	{
		void AddFactoryFromXml(string xml);

		void AddFactory(IWindowsPeripheralFactory factory);

		/// <summary>
		/// When called, tells the peripheral factory collection to update its peripherals
		/// Typically called on StartSettings to add connected peripherals from the whitelist
		/// </summary>
		/// <returns>True if a device was added to the core</returns>
		bool UpdatePeripherals();
	}

	interface IWindowsPeripheralFactoryCollection<TFactory> : IWindowsPeripheralFactoryCollection
		where TFactory : IWindowsPeripheralFactory
	{
		void AddFactory(TFactory factory);
	}
}
