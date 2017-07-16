using TowerDefenseColab.GameObjects.Enemies;

namespace TowerDefenseColab.GameBusHere.Messages
{
    public class EnemyDespawned : IGameMessage
    {
        public Enemy Enemy { get; }

        public EnemyDespawned(Enemy enemy)
        {
            Enemy = enemy;
        }
    }
}