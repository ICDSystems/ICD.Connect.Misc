using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.GlobalCache.FlexApi.RestApi
{
	[JsonConverter(typeof(NetworkConverter))]
	public sealed class Network
	{
		 public bool Dhcp { get; set; }
		 public string Gateway { get; set; }
		 public string IpAddress { get; set; }
		 public string SubnetMask { get; set; }
		 public string PrimaryDnsServer { get; set; }
		 public string SecondaryDnsServer { get; set; }
		 public string NetBiosName { get; set; }
	}

	public sealed class NetworkConverter : AbstractGenericJsonConverter<Network>
	{
		private const string ATTR_DHCP = "dhcp";
		private const string ATTR_GATEWAY = "gateway";
		private const string ATTR_IP_ADDRESS = "ipAddress";
		private const string ATTR_SUBNET_MASK = "subnetMask";
		private const string ATTR_PRIMARY_DNS_SERVER = "primaryDNSServer";
		private const string ATTR_SECONDARY_DNS_SERVER = "secondaryDNSServer";
		private const string ATTR_NET_BIOS_NAME = "netBIOSName";

		/// <summary>
		/// Override to write properties to the writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		/// <param name="serializer"></param>
		protected override void WriteProperties(JsonWriter writer, Network value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			writer.WriteProperty(ATTR_DHCP, value.Dhcp);
			writer.WriteProperty(ATTR_GATEWAY, value.Gateway);
			writer.WriteProperty(ATTR_IP_ADDRESS, value.IpAddress);
			writer.WriteProperty(ATTR_SUBNET_MASK, value.SubnetMask);
			writer.WriteProperty(ATTR_PRIMARY_DNS_SERVER, value.PrimaryDnsServer);
			writer.WriteProperty(ATTR_SECONDARY_DNS_SERVER, value.SecondaryDnsServer);
			writer.WriteProperty(ATTR_NET_BIOS_NAME, value.NetBiosName);
		}

		/// <summary>
		/// Override to handle the current property value with the given name.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="reader"></param>
		/// <param name="instance"></param>
		/// <param name="serializer"></param>
		protected override void ReadProperty(string property, JsonReader reader, Network instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case ATTR_DHCP:
					instance.Dhcp = reader.GetValueAsBool();
					break;
				case ATTR_GATEWAY:
					instance.Gateway = reader.GetValueAsString();
					break;
				case ATTR_IP_ADDRESS:
					instance.IpAddress = reader.GetValueAsString();
					break;
				case ATTR_SUBNET_MASK:
					instance.SubnetMask = reader.GetValueAsString();
					break;
				case ATTR_PRIMARY_DNS_SERVER:
					instance.PrimaryDnsServer = reader.GetValueAsString();
					break;
				case ATTR_SECONDARY_DNS_SERVER:
					instance.SecondaryDnsServer = reader.GetValueAsString();
					break;
				case ATTR_NET_BIOS_NAME:
					instance.NetBiosName = reader.GetValueAsString();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}