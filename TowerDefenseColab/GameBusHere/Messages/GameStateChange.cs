using TowerDefenseColab.GamePhases;
using TowerDefenseColab.GamePhases.GameLevels;

namespace TowerDefenseColab.GameBusHere.Messages
{
    public class GameStateChange : IGameMessage
    {
        public GameLevel GameLevel { get; }
        public GameState GameState { get; }

        public GameStateChange(GameState gameState, GameLevel gameLevel)
        {
            GameLevel = gameLevel;
            GameState = gameState;
        }
    }
}