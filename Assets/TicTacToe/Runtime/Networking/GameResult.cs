namespace TicTacToe.Networking
{
    public interface IGameResult
    {
        
    }

    public class GameOver : IGameResult
    {
        private readonly int _winner;
        private readonly int _looser;

        public GameOver(int winner, int looser)
        {
            _winner = winner;
            _looser = looser;
        }
    }

    public class Draw : IGameResult
    {
        
    }
}