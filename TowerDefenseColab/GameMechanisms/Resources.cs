using TowerDefenseColab.GameObjects;
using TowerDefenseColab.GameObjects.Enemies.Old;

namespace TowerDefenseColab.GameMechanisms
{
    public class Resources
    {
        public decimal Amount { get; private set; }

        public Resources(decimal startAmount)
        {
            Amount = startAmount;
        }

        private void Add(decimal amount)
        {
            Amount += amount;
        }

        public void AddForKilling(EnemyBase enemy)
        {
            // TODO: add proper amount depending on the enemy type/level/other modifiers?
            Add(enemy.ResourcesForKilling);
        }

        /// <summary>
        /// Attempt to take (subtract) the desired amount. Returns true if it was subtracted, otherwise false.
        /// </summary>
        public bool TryTake(decimal amount)
        {
            if (Amount >= amount)
            {
                Add(-amount);
                return true;
            }
            return false;
        }
    }
}
