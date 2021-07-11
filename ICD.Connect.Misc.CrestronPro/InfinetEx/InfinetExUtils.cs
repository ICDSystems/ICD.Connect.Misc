using System;
using ICD.Connect.Misc.CrestronPro.Devices.InfinetExGateway;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
#endif

namespace ICD.Connect.Misc.CrestronPro.InfinetEx
{
	public static class InfinetExUtils
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

#if SIMPLSHARP
		/// <summary>
		/// Determines the correct way to instantiate a cresnet device.
		/// Instantiates on a branch if one is provided, else instantiates on the controlsystem
		/// </summary>
		/// <typeparam name="TInfinetExDevice"></typeparam>
		/// <param name="byteId"></param>
		/// <param name="parentId"></param>
		/// <param name="factory"></param>
		/// <param name="instantiate">Instantiate an infinetEx device on the given gateway</param>
		/// <returns></returns>
		public static TInfinetExDevice InstantiateInfinetExDevice<TInfinetExDevice>(byte byteId, int parentId,
																			  IDeviceFactory factory,
																			  Func<byte, GatewayBase, TInfinetExDevice> instantiate)
		{
			IInfinetExGatewayAdapter gateway = factory.GetOriginatorById<IInfinetExGatewayAdapter>(parentId);
			if (gateway == null)
				throw new ArgumentException(string.Format("Unable to locate InfinetEx Gateway with id {0}", parentId), "parentId");

			GatewayBase gatewayBase = gateway.InfinetExGateway;
			if (gatewayBase == null)
				throw new ArgumentException(string.Format("Unable to get InfinetEx Gateway from device with id {0}", parentId),
				                            "parentId");

			return instantiate(byteId, gateway.InfinetExGateway);
		}
#endif
	}
}