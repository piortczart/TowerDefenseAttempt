using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TowerDefenseColab.GameObjects.Enemies;
using TowerDefenseColab.GamePhases.GameLevels;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;

namespace TowerDefenseColab.GameObjects.Towers
{
    public class TowerBase : GameLoopMethods
    {
        public TowerStateEnum TowerStateEnum = TowerStateEnum.Setup;
        private readonly GameLevelActionLimiter _shootLimiter;
        public readonly TowerSettings Settings;
        private readonly InputManager _inputManager;
        private readonly GameLevel _gameLevel;
        private PointF LocationCenter { get; set; }
        private readonly EntitysSpriteDirected _sprite;
        private readonly GraphicsTracker _graphicsTracker;

        public TowerBase(TowerSettings settings, GameLevelTime gameLevelTime, InputManager inputManager,
            GameLevel gameLevel, GraphicsTracker graphicsTracker, SpriteSheets spriteSheets)
        {
            _shootLimiter = new GameLevelActionLimiter(gameLevelTime, settings.ShootFrequency);
            Settings = settings;
            _inputManager = inputManager;
            _gameLevel = gameLevel;
            _graphicsTracker = graphicsTracker;

            // TODO: Create sprite without direction.
            var spr = new SpriteWithDirections
            {
                Sprites = new Dictionary<SpriteDirectionEnum, SpriteDetails>
                    {
                        {SpriteDirectionEnum.BottomLeft, spriteSheets.GetSprite(SpriteEnum.Tower)},
                        {SpriteDirectionEnum.BottomRight, spriteSheets.GetSprite(SpriteEnum.Tower)},
                        {SpriteDirectionEnum.TopLeft, spriteSheets.GetSprite(SpriteEnum.Tower)},
                        {SpriteDirectionEnum.TopRight, spriteSheets.GetSprite(SpriteEnum.Tower)}
                    }
            };

            _sprite = new EntitysSpriteDirected(spr);
        }

        public override void Init()
        {
        }

        public override void Update(TimeSpan timeDelta)
        {
            switch (TowerStateEnum)
            {
                case TowerStateEnum.Setup:
                    LocationCenter = _inputManager.GetMousePosition();
                    _sprite.IsRelativeToMap = false;
                    break;
                case TowerStateEnum.Active:
                    _sprite.IsRelativeToMap = true;
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
            _sprite.Render(g, _graphicsTracker, LocationCenter);

            //g.Graphics.DrawEllipse(Pens.Bisque, LocationCenter.X - Settings.RangePixels,
            //    LocationCenter.Y - Settings.RangePixels,
            //    Settings.RangePixels * 2, Settings.RangePixels * 2);

            Enemy enemy = GetClosestEnemy();
            if (enemy != null && IsInRange(enemy))
            {
                g.Graphics.DrawLine(Pens.Crimson, LocationCenter.X, LocationCenter.Y, enemy.LocationCenter.X, enemy.LocationCenter.Y);
            }
        }

        private Enemy GetClosestEnemy()
        {
            return _gameLevel
                .CurrentEnemiesNew
                .Where(m => m.IsAlive && m.IsVisible)
                .OrderBy(m => m.GetDistance(this))
                .FirstOrDefault();
        }

        private bool IsInRange(Enemy enemy)
        {
            return enemy.GetDistance(this) < Settings.RangePixels;
        }

        private void Shoot()
        {
            // Find the closest enemy.
            Enemy closestEnemy = GetClosestEnemy();
            if (closestEnemy != null && IsInRange(closestEnemy))
            {
                _shootLimiter.DoingIt();

                closestEnemy.Shot(this);
            }
        }

        public void SetLocationCenter(Point point)
        {
            LocationCenter = point;
        }
    }
}