using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using TowerDefenseColab.GamePhases.GameLevels;

namespace TowerDefenseColab.GameObjects
{
    public class TowerBase : GameObjectBase
    {
        public TowerStateEnum TowerStateEnum = TowerStateEnum.Setup;
        private readonly GameLevelActionLimiter _shootLimiter;
        public readonly TowerSettings Settings;
        private readonly InputManager _inputManager;
        private readonly GameLevel _gameLevel;

        public TowerBase(TowerSettings settings, GameLevelTime gameLevelTime, InputManager inputManager,
            GameLevel gameLevel)
        {
            _shootLimiter = new GameLevelActionLimiter(gameLevelTime, settings.ShootFrequency);
            Settings = settings;
            _inputManager = inputManager;
            _gameLevel = gameLevel;
        }

        public override void Init()
        {
            Sprite = GraphicsHelper.ResizeImage(Image.FromFile("Assets/Towers/tower_29.png"), 30, 30);
        }

        public override void Update(TimeSpan timeDelta)
        {
            switch (TowerStateEnum)
            {
                case TowerStateEnum.Setup:
                    LocationCenter = _inputManager.GetMousePosition();
                    break;
                case TowerStateEnum.Active:
                    if (_shootLimiter.CanDoStuff())
                    {
                        Shoot();
                    }
                    break;
                default:
                    throw new IndexOutOfRangeException(nameof(TowerStateEnum));
            }
        }

        public override void Render(BufferedGraphics g)
        {
            base.Render(g);

            g.Graphics.DrawEllipse(Pens.Bisque, LocationCenter.X - Settings.RangePixels,
                LocationCenter.Y - Settings.RangePixels,
                Settings.RangePixels * 2, Settings.RangePixels * 2);

            var enemy = GetClosestEnemy();
            if (enemy != null && IsInRange(enemy))
            {
                g.Graphics.DrawLine(Pens.Crimson, LocationCenter.X, LocationCenter.Y, enemy.LocationCenter.X, enemy.LocationCenter.Y);
            }
        }

        private EnemyBase GetClosestEnemy()
        {
            return _gameLevel
                .CurrentMonsters
                .Where(m => m.IsAlive && m.IsVisible)
                .OrderBy(m => m.GetDistance(this))
                .FirstOrDefault();
        }

        private bool IsInRange(EnemyBase enemy)
        {
            return enemy.GetDistance(this) < Settings.RangePixels;
        }

        private void Shoot()
        {
            // Find the closest enemy.
            EnemyBase closestEnemy = GetClosestEnemy();
            if (closestEnemy != null && IsInRange(closestEnemy))
            {
                _shootLimiter.DoingIt();

                closestEnemy.Shot(this);
            }
        }
    }
}