using System;
using System.Collections.Generic;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.Windows.Devices.WindowsPeripheralDevice;

namespace ICD.Connect.Misc.Windows.WindowsPeripheral.Factories
{
	public sealed class UsbWindowsPeripheralFactory : AbstractWindowsPeripheralFactory<UsbWindowsPeripheralDevice>
	{
		private const string IDS_ELEMENT = "Ids";
		private const string ID_ELEMENT = "Id";

		private static readonly Guid s_UsbPeripheralGuidSeed = new Guid("9c47b703-ea07-4e11-aacd-8721a7a64a21");

		private readonly IcdHashSet<UsbProductInfo> m_Products;

		public UsbWindowsPeripheralFactory()
		{
			m_Products = new IcdHashSet<UsbProductInfo>();
		}

		public bool ContainsProductInfo(UsbProductInfo productInfo)
		{
			return m_Products.Contains(productInfo);
		}

		public IEnumerable<UsbProductInfo> GetProductInfos()
		{
			return m_Products.ToArray(m_Products.Count);
		}

		private void AddProductInfoRange(IEnumerable<UsbProductInfo> productInfos)
		{
			m_Products.AddRange(productInfos);
		}

		/// <summary>
		/// Returns a guid for the specific peripheral type
		/// Used to prevent possible collisions of seeded guids
		/// </summary>
		public override Guid PeripheralTypeGuid { get { return s_UsbPeripheralGuidSeed; } }

		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			IEnumerable<UsbProductInfo> productInfos = XmlUtils.ReadListFromXml(xml, IDS_ELEMENT, ID_ELEMENT, UsbProductInfoExtensions.FromXml);

			AddProductInfoRange(productInfos);
		}

		/// <summary>
		/// Override for any factory-specific instantiation that needs to happen.
		/// </summary>
		/// <param name="device"></param>
		protected override void InstantiateDeviceFinal(UsbWindowsPeripheralDevice device)
		{
			base.InstantiateDeviceFinal(device);

			device.UpdateCimInstance();
		}
	}
}
