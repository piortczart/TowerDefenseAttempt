using TowerDefenseColab.GamePhases;

namespace TowerDefenseColab.GameBusHere.Messages
{
    public class GameStateChange : IGameMessage
    {
        public GameState GameState { get; }

        public GameStateChange(GameState gameState)
        {
            GameState = gameState;
        }
    }
}