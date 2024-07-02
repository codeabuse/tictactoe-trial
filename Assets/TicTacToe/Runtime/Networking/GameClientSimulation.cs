using System;
using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using TicTacToe.Structures;
using UnityEngine;

namespace TicTacToe.Networking
{
    [Serializable]
    public class GameClientSimulation : IGameClient
    {
        [SerializeField]
        private NetworkSimulation simulation;

        public IPlayerMoveRequest CreateMoveRequest()
        {
            return new PlayerMoveRequestSimulation(simulation);
        }

        public IGameStartRequest CreateGameStartRequest()
        {
            return new GameStartRequestSimulation(simulation);
        }

        public IEndGameRequest CreateEndGameRequest()
        {
            return new GameEndRequestSimulation(simulation);
        }
    }

    public class GameEndRequestSimulation : IEndGameRequest
    {
        private readonly NetworkSimulation _simulation;

        public GameEndRequestSimulation(NetworkSimulation simulation)
        {
            _simulation = simulation;
        }

        public IEndGameResponse Send(IGameResult result)
        {
            return new GameEndResponseSimulation(result, _simulation);
        }
    }

    public class GameEndResponseSimulation : IEndGameResponse
    {
        private readonly IGameResult _result;
        private readonly NetworkSimulation _simulation;

        public GameEndResponseSimulation(IGameResult result, NetworkSimulation simulation)
        {
            _result = result;
            _simulation = simulation;
        }

        public async UniTask<ResultVoid<NetworkError>> GetResponse()
        {
            var result = await _simulation.TryWaitResponse(_simulation.SimulateDelay);
            result.Map(() => NetworkSimulationLog.LogAction(_result.ToString()),
                    e => NetworkSimulationLog.LogAction(e));
            return result;
        }
    }

    public class GameStartRequestSimulation : IGameStartRequest
    {
        private readonly NetworkSimulation _simulation;

        public GameStartRequestSimulation(NetworkSimulation simulation)
        {
            _simulation = simulation;
        }

        public IGameStartResponse Send(PlayerEntity[] players)
        {
            return new GameStartResposneSimulation(players, _simulation);
        }
    }

    public class GameStartResposneSimulation : IGameStartResponse
    {
        private readonly PlayerEntity[] _players;
        private readonly NetworkSimulation _networkSimulation;

        public GameStartResposneSimulation(PlayerEntity[] players, NetworkSimulation networkSimulation)
        {
            _players = players;
            _networkSimulation = networkSimulation;
            NetworkSimulationLog.NewLog();
        }

        public async UniTask<ResultVoid<NetworkError>> GetResponse()
        {
            var result = await _networkSimulation.TryWaitResponse(_networkSimulation.SimulateDelay);
            result.Map(() =>
                    {
                        NetworkSimulationLog.LogAction($"start {_players[0].Name} X - O {_players[1].Name}");
                    },
                    error =>
                    {
                        NetworkSimulationLog.LogAction(error);
                    });
            return result;
        }
    }

    public class PlayerMoveRequestSimulation : IPlayerMoveRequest
    {
        private readonly NetworkSimulation _simulation;

        public PlayerMoveRequestSimulation(NetworkSimulation simulation)
        {
            _simulation = simulation;
        }

        public IPlayerMoveResponse Send(PlayerMove move)
        {
            return new PlayerMoveResponseSimulation(move, _simulation);
        }
    }

    public class PlayerMoveResponseSimulation : IPlayerMoveResponse
    {
        private readonly PlayerMove _requestedMove;
        private readonly NetworkSimulation _networkSimulation;

        public PlayerMoveResponseSimulation(PlayerMove requestedMove, NetworkSimulation networkSimulation)
        {
            _requestedMove = requestedMove;
            _networkSimulation = networkSimulation;
        }
        
        public async UniTask<ResultVoid<NetworkError>> GetResponse()
        {
            var result = await _networkSimulation.TryWaitResponse(_networkSimulation.SimulateDelay);
            result.Map(() =>
                    {
                        NetworkSimulationLog.LogAction(_requestedMove.ToString());
                    },
                    error =>
                    {
                        NetworkSimulationLog.LogAction(error);
                    });
            return result;
        }
    }
}