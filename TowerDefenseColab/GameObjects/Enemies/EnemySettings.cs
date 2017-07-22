using System.Collections.Generic;
using System.Drawing;

namespace TowerDefenseColab.GameObjects.Enemies
{
    public class EnemySettings
    {
        public EnemyTypeEnum EnemyType { get; }
        public List<Point> Waypoints { get; }
        public float Health { get; }

        public EnemySettings(EnemyTypeEnum enemyType, List<Point> waypoints, float health)
        {
            EnemyType = enemyType;
            Waypoints = waypoints;
            Health = health;
        }
    }
}