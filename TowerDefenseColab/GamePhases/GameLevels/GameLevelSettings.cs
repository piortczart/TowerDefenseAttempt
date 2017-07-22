using System;
using System.Collections.Generic;
using System.Drawing;
using TowerDefenseColab.GameObjects.Enemies;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    public class GameLevelSettings
    {
        public int LevelNumber { get; set; }
        public TimeSpan SpawnFrequency { get; set; }
        public IEnumerable<EnemyTypeEnum> EnemyTypesToSpawn { get; set; }
        public List<Point> Waypoints { get; set; }
        //public Point SpawnPoint { get { return Waypoints[0]; } }
        public decimal StartingResources { get; set; }
        public LevelMap Map { get; set; }
        public GamePhaseEnum PhaseEnum { get; set; }
    }
}