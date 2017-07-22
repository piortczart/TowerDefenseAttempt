using System;
using System.Collections.Generic;
using System.Drawing;
using StructureMap;

namespace TowerDefenseColab.GameObjects.Enemies
{
    public class EnemyFactory
    {
        private readonly IContainer _container;
        private const float BlueVanHealth = 10;
        public EnemyFactory(IContainer container)
        {
            _container = container;
        }

        public Enemy CreateDefault(EnemyTypeEnum enemyType, List<Point> waypoints)
        {
            float health;
            switch (enemyType)
            {
                case EnemyTypeEnum.BlueVan:
                    health = BlueVanHealth;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
            }

            EnemySettings enemySettings = new EnemySettings(enemyType, waypoints, health);
            return Create(enemySettings);
        }

        public Enemy Create(EnemySettings enemySettings)
        {
            if (enemySettings.EnemyType != EnemyTypeEnum.BlueVan)
            {
                throw new NotImplementedException();
            }

            return _container.With("settings").EqualTo(enemySettings).GetInstance<Enemy>();
        }
    }
}