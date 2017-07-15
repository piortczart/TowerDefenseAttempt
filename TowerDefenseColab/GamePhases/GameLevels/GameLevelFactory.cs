using StructureMap;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    public class GameLevelFactory
    {
        private readonly IContainer _container;

        public GameLevelFactory(IContainer container)
        {
            _container = container;
        }

        public GameLevel CreateLevel(GameLevelSettings levelSettings)
        {
            return _container.With("settings").EqualTo(levelSettings).GetInstance<GameLevel>();
        }
    }
}