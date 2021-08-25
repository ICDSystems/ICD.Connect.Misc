#if NETFRAMEWORK
extern alias RealNewtonsoft;
using RealNewtonsoft.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses.Converters
{
	public sealed class ListPackageResponseConverter : AbstractVibeResponseConverter<ListPackageResponse, PackageData[]>
	{
	}

	public sealed class PackageDataConverter : AbstractGenericJsonConverter<PackageData>
	{
		private const string PROP_PACKAGE_NAME = "packageName";
		private const string PROP_IS_SYSTEM = "isSystem";
		private const string PROP_SOURCE_DIR = "sourceDir";

		protected override void WriteProperties(JsonWriter writer, PackageData value, JsonSerializer serializer)
		{
			base.WriteProperties(writer, value, serializer);

			if (value.PackageName != null)
				writer.WriteProperty(PROP_PACKAGE_NAME, value.PackageName);

			if (value.IsSystem)
				writer.WriteProperty(PROP_IS_SYSTEM, value.IsSystem);

			if (value.SourceDirectory != null)
				writer.WriteProperty(PROP_SOURCE_DIR, value.SourceDirectory);
		}

		protected override void ReadProperty(string property, JsonReader reader, PackageData instance, JsonSerializer serializer)
		{
			switch (property)
			{
				case PROP_PACKAGE_NAME:
					instance.PackageName = reader.GetValueAsString();
					break;

				case PROP_IS_SYSTEM:
					instance.IsSystem = reader.GetValueAsBool();
					break;

				case PROP_SOURCE_DIR:
					instance.SourceDirectory = reader.GetValueAsString();
					break;

				default:
					base.ReadProperty(property, reader, instance, serializer);
					break;
			}
		}
	}
}
