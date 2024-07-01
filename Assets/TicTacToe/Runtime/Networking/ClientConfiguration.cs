using UnityEngine;

namespace TicTacToe.Networking
{
    [CreateAssetMenu(fileName = "Client Configuration", menuName = "TicTacToe/Client Configuration")]
    public class ClientConfiguration : ScriptableObject
    {
        [SerializeReference, Instantiate(typeof(IGameClient))]
        private IGameClient _clientBackend;

        public IGameClient GetClientBackend()
        {
            return _clientBackend;
        }
    }
}