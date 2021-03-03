using System;
using ICD.Connect.Misc.Windows.WindowsPeripheral.Factories;

namespace ICD.Connect.Misc.Windows.WindowsPeripheral.FactoryCollections
{
	interface IWindowsPeripheralFactoryCollection : IDisposable
	{
		void AddFactoryFromXml(string xml);

		void AddFactory(IWindowsPeripheralFactory factory);

		/// <summary>
		/// When called, tells the peripheral factory collection to update its peripherals
		/// Typically called on StartSettings to add connected peripherals from the whitelist
		/// </summary>
		void UpdatePeripherals();
	}

	interface IWindowsPeripheralFactoryCollection<TFactory> : IWindowsPeripheralFactoryCollection
		where TFactory : IWindowsPeripheralFactory
	{
		void AddFactory(TFactory factory);
	}
}
