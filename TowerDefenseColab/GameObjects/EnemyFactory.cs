using System;
using System.Collections.Generic;
using System.Drawing;
using TowerDefenseColab.GameMechanisms;
using TowerDefenseColab.GamePhases;

namespace TowerDefenseColab.GameObjects
{
    public class EnemyFactory
    {
        private readonly GamePhaseManager _gamePhaseManager;

        public EnemyFactory(GamePhaseManager gamePhaseManager)
        {
            _gamePhaseManager = gamePhaseManager;
        }

        public EnemyBase GetEnemy(EnemyTypeEnum type)
        {
            switch (type)
            {
                case EnemyTypeEnum.CircleOfDeath:
                    return new EnemyBase(
                        new EnemySettings()
                        {
                            Health = 3,
                            Speed = 100,
                            ResourcesForKilling = 1
                        }, 
                        "Assets\\sprite1.png",
                        new AnimationInfo()
                        {
                            FrameSize = new Size(32, 32),
                            AnimationsList = new List<Animation>()
                            {
                                new Animation(0, 3, 20f)
                            }
                        });
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}