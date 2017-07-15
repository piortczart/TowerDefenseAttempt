using StructureMap;
using TowerDefenseColab.GamePhases.GameLevels;

namespace TowerDefenseColab.GameObjects
{
    public class TowerFactory
    {
        readonly IContainer _container;

        public TowerFactory(IContainer container)
        {
            _container = container;
        }

        public TowerBase GetTower(GameLevelTime time, GameLevel gameLevel, TowerSettings towerSettings)
        {
            return _container
                .With("gameLevelTime").EqualTo(time)
                .With("gameLevel").EqualTo(gameLevel)
                .With("settings").EqualTo(towerSettings)
                .GetInstance<TowerBase>();
        }
    }
}