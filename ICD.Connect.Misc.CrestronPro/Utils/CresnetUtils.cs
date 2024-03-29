﻿#if !NETSTANDARD
using ICD.Connect.Settings;
using System;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge;

namespace ICD.Connect.Misc.CrestronPro.Utils
{
	public static class CresnetUtils
	{
		private const byte MIN_ID = 0x03;
		private const byte MAX_ID = 0xFE;

		/// <summary>
		/// Returns true if the given cresnet id is in a valid range.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool IsValidId(byte id)
		{
			return id >= MIN_ID && id <= MAX_ID;
		}

		/// <summary>
		/// Determines the correct way to instantiate a cresnet device.
		/// Instantiates on a branch if one is provided, else instantiates on the controlsystem
		/// </summary>
		/// <typeparam name="TCresnetDevice"></typeparam>
		/// <param name="byteId"></param>
		/// <param name="branchId"></param>
		/// <param name="bridgeId"></param>
		/// <param name="factory"></param>
		/// <param name="noBranchInstantiate">Instantiate a cresnet device directly attached to the controlsystem</param>
		/// <param name="bridgeInstantiate">Instantiate a cresnet device attached to a bridge</param>
		/// <returns></returns>
		public static TCresnetDevice InstantiateCresnetDevice<TCresnetDevice>(byte byteId, int? branchId, int? bridgeId,
		                                                                      IDeviceFactory factory,
		                                                                      Func<byte, TCresnetDevice> noBranchInstantiate,
		                                                                      Func<byte, CresnetBranch, TCresnetDevice> bridgeInstantiate)
		{
			if (bridgeId == null || branchId == null)
				return noBranchInstantiate(byteId);

			ICresnetBridgeAdapter bridge = factory.GetOriginatorById<ICresnetBridgeAdapter>(bridgeId.Value);
			if (bridge == null)
				throw new ArgumentException(string.Format("Unable to locate bridge with id {0}", bridgeId), "bridgeId");

			CresnetBranch branch = bridge.Branches.FirstOrDefault(b => b.Number == branchId);
			if (branch == null)
				throw new ArgumentException(string.Format("Bridge {0} does not have a branch numbered {1}.", bridgeId, branchId), "branchId");

			return bridgeInstantiate(byteId, branch);
		}
	}
}
#endif
