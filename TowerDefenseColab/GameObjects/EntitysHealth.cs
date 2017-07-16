namespace TowerDefenseColab.GameObjects
{
    public class EntitysHealth
    {
        public float Health { get; set; }
        public bool IsDead => Health <= 0;

        public bool Decrease(float decreaseBy)
        {
            Health -= decreaseBy;
            return IsDead;
        }
    }
}