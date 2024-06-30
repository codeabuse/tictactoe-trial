namespace TicTacToe.StaticData
{
    public static class GameText
    {
        public const string PlayerReadyButtonText = "Let's go!";

        public static string GetPlayerReadyText(string playerName) => string.Format("{0}, your turn.", playerName);
    }
}