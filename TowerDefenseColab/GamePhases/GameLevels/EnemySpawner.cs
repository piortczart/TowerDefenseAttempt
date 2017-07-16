using System;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    public class EnemySpawner
    {
        private TimeSpan _lastSpawn = TimeSpan.MinValue;
        private readonly GameLevelTime _time;
        private readonly TimeSpan _spawnFrequency;

        public EnemySpawner(GameLevelTime time, TimeSpan spawnFrequency)
        {
            _time = time;
            _spawnFrequency = spawnFrequency;
        }

        public bool SpawnEnemy()
        {
            TimeSpan nao = _time.GetCurrent();
            bool isFirstSpawn = _lastSpawn == TimeSpan.MinValue;
            // Spawn enemy if no enemy was spawned yet or if the time since last spawn is long enough.
            bool shouldSpawnEnemy = _lastSpawn + _spawnFrequency <= nao || isFirstSpawn;
            _lastSpawn = nao;
            return shouldSpawnEnemy;
        }
    }
}