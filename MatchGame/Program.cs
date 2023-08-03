namespace MatchGame;

internal static class Program
{
	public static void Main(string[] args)
	{
		using var game = new GameInstance();

		game.Run();
	}
}