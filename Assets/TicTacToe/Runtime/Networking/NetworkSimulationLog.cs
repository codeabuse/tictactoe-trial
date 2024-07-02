namespace TicTacToe.Networking
{
    internal static class NetworkSimulationLog
    {
        private static TextLog _log;
        public static void NewLog()
        {
            _log?.Close();
            _log = new TextLog("game");
        }
        
        public static void LogAction(string action)
        {
            _log.AppendLine(action);
        }
    }
}