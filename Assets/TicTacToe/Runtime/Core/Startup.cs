using Cysharp.Threading.Tasks;
using TicTacToe.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TicTacToe.Core
{
    public class Startup : MonoBehaviour
    {
        [SerializeField]
        private ClientConfiguration _clientConfiguration;
        [SerializeField]
        private int _gameSceneIndex = 1;

        private async void Start()
        {
            GameClient.ApplyConfiguration(_clientConfiguration);
            await SceneManager.LoadSceneAsync(_gameSceneIndex);
        }
    }
}