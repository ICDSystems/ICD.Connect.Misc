using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.Keypads.MockKeypad
{
	[KrangSettings("MockKeypad", typeof(MockKeypad))]
	public sealed class MockKeypadSettings : AbstractKeypadDeviceSettings, IMockKeypadSettings
	{
	}
}
