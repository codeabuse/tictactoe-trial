namespace TicTacToe.Networking
{
    public enum ServerResponse
    {
        OK = 0,
        InvalidMove = 1 << 0,
        InvalidPlayer = 1 << 1
    }
}