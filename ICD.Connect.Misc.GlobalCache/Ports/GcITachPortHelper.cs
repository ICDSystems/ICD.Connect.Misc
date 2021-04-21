using System;
using ICD.Common.Utils;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.GlobalCache.Devices.ITachFlex;
using ICD.Connect.Misc.GlobalCache.FlexApi.RestApi;

namespace ICD.Connect.Misc.GlobalCache.Ports
{
	public static class GcITachPortHelper
	{
		public static void SetModuleType(IGcITachPort instance, Module.eId id, Module.eClass @class, Module.eType type)
		{
			GcITachFlexDevice flexDevice = instance.Device as GcITachFlexDevice;
			if (flexDevice == null)
				return;

			Module module = new Module
			{
				Id = id,
				Class = @class,
				Type = type
			};

			string localUrl = string.Format("api/host/modules/{0}", instance.Module);

			try
			{
				flexDevice.Post(localUrl, module.Serialize());
			}
			catch (Exception e)
			{
				IcdErrorLog.Error("Failed to set module type", e);
			}
		}

		public static void BuildConsoleStatus(IGcITachPort instance, AddStatusRowDelegate addRow)
		{
			addRow("Device", instance.Device);
			addRow("Module", instance.Module);
			addRow("Address", instance.Address);
		}
	}
}