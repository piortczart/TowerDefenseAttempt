using TowerDefenseColab.GameObjects.Enemies;

namespace TowerDefenseColab.GameBusHere.Messages
{
    public class EnemyReachedGoal : IGameMessage
    {
        public Enemy Enemy { get; }

        public EnemyReachedGoal(Enemy enemy)
        {
            Enemy = enemy;
        }
    }
}