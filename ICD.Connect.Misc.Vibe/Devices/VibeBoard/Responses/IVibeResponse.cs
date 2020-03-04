namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	public interface IVibeResponse
	{
		string Type { get; }
		string ResultId { get; }
		bool Sync { get; }
		string ErrorId { get; }
		ErrorData Error { get; }
	}
}
