using System;
using TicTacToe.Gameplay;

namespace TicTacToe.StaticData
{
    public static class DefaultSettings
    {
        public const int GameFieldWidth = 9;
        public const int GameFieldHeight = 10;
        public const int WinningLineLength = 5;
        public const int TurnTime = 20;
        public const float BotTurnMinIdle = 1.5f;
        public const float BotTurnMaxIdle = 5f;

        public static readonly RuleSet DefaultRules = new (
                new(GameFieldWidth, GameFieldHeight),
                WinningLineLength,
                TimeSpan.FromSeconds(TurnTime));
    }
}