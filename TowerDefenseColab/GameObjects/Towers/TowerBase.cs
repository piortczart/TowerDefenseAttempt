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
        private readonly GameLevel _gameLevel;
        private PointF LocationCenter { get; set; }
        private readonly EntitysSpriteDirected _sprite;
        private readonly GraphicsTracker _graphicsTracker;
        private readonly InputManager _inputManager;

        public TowerBase(TowerSettings settings, GameLevelTime gameLevelTime,
            GameLevel gameLevel, GraphicsTracker graphicsTracker, SpriteSheets spriteSheets, InputManager inputManager)
        {
            _shootLimiter = new GameLevelActionLimiter(gameLevelTime, settings.ShootFrequency);
            Settings = settings;
            _gameLevel = gameLevel;
            _graphicsTracker = graphicsTracker;
            _inputManager = inputManager;

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
            // At the start it should stick to the cursor.
            _sprite.IsRelativeToMap = false;
        }

        public override void Update(TimeSpan timeDelta)
        {
            switch (TowerStateEnum)
            {
                case TowerStateEnum.Setup:
                    SetLocationCenter(Point.Add(_inputManager.GetMousePosition(), _sprite.Size.Invert()));
                    break;
                case TowerStateEnum.Active:
                    _sprite.IsRelativeToMap = true;
                    if (_shootLimiter.CanDoStuff())
                    {
                        Shoot();
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Render(BufferedGraphics g)
        {
            _sprite.Render(g, _graphicsTracker, LocationCenter);

            Enemy enemy = GetClosestEnemy();
            if (enemy != null && IsInRange(enemy))
            {
                var realCenter = _sprite.GetWindowLocation(_graphicsTracker, LocationCenter);

                g.Graphics.DrawLine(Pens.Crimson, realCenter, enemy.SpriteDirected.GetWindowLocation(_graphicsTracker, enemy.LocationCenter));
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

        public void Place()
        {
            TowerStateEnum = TowerStateEnum.Active;
            _sprite.IsRelativeToMap = true;
            LocationCenter = _graphicsTracker.ConvertWindowCoordsToMapCoords(LocationCenter);
        }
    }
}