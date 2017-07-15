using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using TowerDefenseColab.GameMechanisms;

namespace TowerDefenseColab.GameObjects
{
    public class EnemyBase : AnimatedSprite
    {
        /// <summary>
        /// Pixels per second.
        /// </summary>
        protected float Speed { private get; set; }

        protected float Health { get; set; } = 1;

        public bool IsAlive { get; private set; } = true;

        public bool IsVisible { get; private set; } = true;

        public TimeSpan AgonyPeriod { get; private set; }

        public List<Point> Waypoints { get; set; }

        private PointF currentTarget { get; set; }

        private int currentTargetIndex { get; set; } = 0;

        public decimal ResourcesForKilling { get; protected set; }

        public Action<EnemyBase> OnDeathAction { get; set; }

        /// <summary>
        /// Once the monster reaches the last waypoint, this will be true.
        /// </summary>
        public bool FoundPointG { get; private set; }

        public EnemyBase(EnemySettings enemySettings, string spritePath, AnimationInfo animInfo)
        {
            Sprite = Image.FromFile(spritePath);
            Health = enemySettings.Health;
            Speed = enemySettings.Speed;
            ResourcesForKilling = enemySettings.ResourcesForKilling;
            FrameSize = animInfo.FrameSize;
            Animations = animInfo.AnimationsList;
            CurrentAnimation = Animations.FirstOrDefault();
        }

        public override void Init()
        {
            FoundPointG = false;
        }

        public override void Update(TimeSpan timeDelta)
        {
            // Already dead?
            if (!IsAlive)
            {
                // Decrease it's agony timer.
                AgonyPeriod -= timeDelta;
                if (AgonyPeriod <= TimeSpan.Zero)
                {
                    // Agnony ended? Hide/remove him.
                    IsVisible = false;
                }
                return;
            }

            //var deltaX = Speed*(float) timeDelta.TotalSeconds;
            float deltaX = 0;
            float deltaY = 0;

            //get current waypoint
            currentTarget = Waypoints[currentTargetIndex];

            var monsterLocation = new Vector2(LocationCenter.X, LocationCenter.Y);
            var target = new Vector2(currentTarget.X, currentTarget.Y);

            //calculate distance
            var distance = Vector2.Distance(monsterLocation, target);

            //check if monster is near target X for +- 1 pixel
            if (target.X - 1 <= monsterLocation.X && target.X + 1 >= monsterLocation.X)
            {
                deltaY = Speed * (float)timeDelta.TotalSeconds;
                //deltaY = Speed * (float)0.0124125;
            }
            //check if monster is near target Y for +- 1 pixel
            if (target.Y - 1 <= monsterLocation.Y && target.Y + 1 >= monsterLocation.Y)
            {
                deltaX = Speed * (float)timeDelta.TotalSeconds;
                //deltaX = Speed * (float)0.0124125;
            }

            //if monster distance is more equal 1 pixel to target
            if (distance >= 1)
            {
                //if monster distance to target is about 4 - 0,5 pixels then deltaX/Y divide by 2 or may be set for const int
                if (distance <= 4 && distance >= 0.5)
                {
                    deltaX /= 2;
                    deltaY /= 2;
                }

                if (target.Y > Math.Round(monsterLocation.Y))
                    LocationCenter = new PointF(LocationCenter.X + deltaX, LocationCenter.Y + deltaY);
                else if (target.Y < Math.Round(monsterLocation.Y))
                    LocationCenter = new PointF(LocationCenter.X + deltaX, LocationCenter.Y - deltaY);
                else if (target.X > Math.Round(monsterLocation.X))
                    LocationCenter = new PointF(LocationCenter.X + deltaX, LocationCenter.Y + deltaY);
                else if (target.X < Math.Round(monsterLocation.X))
                    LocationCenter = new PointF(LocationCenter.X - deltaX, LocationCenter.Y + deltaY);
            }
            else
            {
                //if monster distance is less than 1 pixel to target increment waypoints index
                currentTargetIndex++;

                // Monster reached the last waypoint - he dies, but a flag is raised.
                if (currentTargetIndex >= Waypoints.Count)
                {
                    FoundPointG = true;
                    Die();
                }
            }

            //TODO: need to check if map end is at axis Y
            // Because of a buggy paths I needed to add this.
            if (LocationCenter.X > 800)
            {
                FoundPointG = true;
                Die();
            }

            base.Update(timeDelta);
        }

        private void Die()
        {
            IsAlive = false;

            ShowDeathSprite();

            // How long will this enemy will be visible after death.
            AgonyPeriod = CurrentAnimation.AnimationTime;

            OnDeathAction(this);
        }

        private void ShowDeathSprite()
        {
            Sprite = Image.FromFile("Assets\\boom.png");
            FrameSize = new Size(60, 45);
            Animations = new List<Animation>()
            {
                new Animation(0, 3, 10f)
            };
            CurrentAnimation = Animations[0];
        }

        // Thie enemy was shot by the tower.
        public void Shot(TowerBase towerBase)
        {
            Health -= towerBase.Settings.Powah;
            if (Health <= 0)
            {
                Die();
            }
        }

        public override void Render(BufferedGraphics g)
        {
            base.Render(g);

            g.Graphics.DrawString($"{Health}", new Font("monospace", 10),
                new SolidBrush(Color.Blue), LocationCenter.X, LocationCenter.Y - 10);
        }
    }
}