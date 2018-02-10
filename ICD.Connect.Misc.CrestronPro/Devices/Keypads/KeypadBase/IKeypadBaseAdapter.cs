using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads
{
	public interface IKeypadBaseAdapter
	{
		
		//TODO: Roll Down to Our Abstraction
		event EventHandler<ButtonEventArgs> OnButtonStateChange;
	}
}