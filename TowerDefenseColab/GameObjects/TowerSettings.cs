using System;

namespace TowerDefenseColab.GameObjects
{
    public class TowerSettings
    {
        public TimeSpan ShootFrequency { get; set; }
        public float RangePixels { get; set; }
        public float Powah { get; set; }
        public decimal CostBase { get; set; }
    }
}