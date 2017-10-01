using ICD.Connect.Misc.GlobalCache.FlexApi;
using NUnit.Framework;

namespace ICD.Connect.Misc.GlobalCache.Tests.FlexApi
{
	[TestFixture]
    public sealed class FlexDataTest
    {
		[TestCase("test")]
		public void CommandTest(string command)
		{
			FlexData data = new FlexData(command, 0, 0);
			Assert.AreEqual(command, data.Command);
		}

		[TestCase((uint)1)]
		public void ModuleTest(uint module)
		{
			FlexData data = new FlexData("test", module, 0);
			Assert.AreEqual(module, data.Module);
		}

		[TestCase((uint)1)]
		public void PortTest(uint port)
		{
			FlexData data = new FlexData("test", 0, port);
			Assert.AreEqual(port, data.Port);
		}

		[TestCase(5)]
		public void ParameterCountTest(int count)
		{
			FlexData data = new FlexData("test", 0, 0, new object[count]);
			Assert.AreEqual(count, data.ParameterCount);
		}

		[TestCase("ERR IO006\r", true)]
		[TestCase("test,0:0\r", false)]
		public void IsErrorTest(string response, bool expected)
		{
			FlexData data = FlexData.Deserialize(response);
			Assert.AreEqual(expected, data.IsError);
		}

		[TestCase("ERR IO006\r", "IO006")]
		public void ErrorCodeTest(string response, string expected)
		{
			FlexData data = FlexData.Deserialize(response);
			Assert.AreEqual(expected, data.ErrorCode);
		}

		[Test]
		public void DeserializeTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void SerializeTest()
		{
			Assert.Inconclusive();
		}
	}
}
