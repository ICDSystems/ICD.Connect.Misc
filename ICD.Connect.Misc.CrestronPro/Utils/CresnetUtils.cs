namespace ICD.Connect.Misc.CrestronPro.Utils
{
	public static class CresnetUtils
	{
		public const byte MIN_ID = 0x03;
		public const byte MAX_ID = 0xFE;

		/// <summary>
		/// Returns true if the given cresnet id is in a valid range.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool IsValidId(byte id)
		{
			return id >= MIN_ID && id <= MAX_ID;
		}
	}
}
