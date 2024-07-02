namespace TicTacToe.Networking
{
    public class NetworkError
    {
        public string Message { get; }
        public NetworkError(string message)
        {
            Message = message;
        }

        public static implicit operator string(NetworkError error) => error.Message;
    }
}