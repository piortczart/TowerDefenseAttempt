using StructureMap;
using TowerDefenseColab.GamePhases.GameLevels;
using TowerDefenseColab.GraphicsPoo;

namespace TowerDefenseColab.GameObjects.Towers
{
    public class TowerFactory
    {
        private readonly IContainer _container;

        public TowerFactory(IContainer container)
        {
            _container = container;
        }

        public TowerBase GetTower(GameLevelTime time, GameLevel gameLevel, TowerSettings towerSettings, GraphicsTracker graphicsTracker)
        {
            return _container
                .With("gameLevelTime").EqualTo(time)
                .With("gameLevel").EqualTo(gameLevel)
                .With("settings").EqualTo(towerSettings)
                .With("graphicsTracker").EqualTo(graphicsTracker)
                .GetInstance<TowerBase>();
        }
    }
}