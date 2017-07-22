using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;
using TowerDefenseColab.GameObjects.Towers;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;

namespace TowerDefenseColab.GameObjects.Enemies
{
    public class Enemy : GameLoopMethods
    {
        private readonly GraphicsTracker _graphicsTracker;
        private readonly List<Point> _waypoints;
        private int _currentWaypoint;
        private float _currentX, _currentY;
        private float speed = 50;
        public PointF LocationCenter { get; private set; }
        private bool FoundPointG { get; set; }
        private readonly GameBus _bus;
        private readonly FontsAndColors _fontsAndColors;
        private readonly EntitysHealth _health;
        private readonly SpriteSheets _spriteSheets;
        public EntitysSpriteDirected SpriteDirected { get; }

        public bool IsAlive => !_health.IsDead;
        public bool IsVisible { get; private set; } = true;
        private TimeSpan AgonyPeriod { get; set; } = TimeSpan.FromSeconds(2);

        public Enemy(
            EnemySettings settings,
            GraphicsTracker graphicsTracker,
            GameBus bus,
            FontsAndColors fontsAndColors,
            SpriteSheets spriteSheets)
        {
            _graphicsTracker = graphicsTracker;
            _bus = bus;
            _fontsAndColors = fontsAndColors;
            _spriteSheets = spriteSheets;
            _health = new EntitysHealth { Health = settings.Health };
            _waypoints = settings.Waypoints;

            SpriteDirected = CreateSprite(settings.EnemyType);
        }

        public override void Init()
        {
        }

        private EntitysSpriteDirected CreateSprite(EnemyTypeEnum settingsEnemyType)
        {
            var spriteWithDirections = new SpriteWithDirections
            {
                Sprites = new Dictionary<SpriteDirectionEnum, SpriteDetails>
                    {
                        {SpriteDirectionEnum.BottomLeft, _spriteSheets.GetSprite(SpriteEnum.VehicleVanBottomLeft)},
                        {SpriteDirectionEnum.BottomRight, _spriteSheets.GetSprite(SpriteEnum.VehicleVanBottomRight)},
                        {SpriteDirectionEnum.TopLeft, _spriteSheets.GetSprite(SpriteEnum.VehicleVanTopLeft)},
                        {SpriteDirectionEnum.TopRight, _spriteSheets.GetSprite(SpriteEnum.VehicleVanTopRight)}
                    }
            };
            return new EntitysSpriteDirected(spriteWithDirections);
        }

        private Point ActualLocationByMap(int mapX, int mapY)
        {
            Point mapLocation = GraphicsHelper.ConvertMapToReal(mapX, mapY, Point.Empty);
            return new Point(mapLocation.X, mapLocation.Y - 34);
        }

        public override void Update(TimeSpan timeDelta)
        {
            if (!IsVisible)
            {
                return;
            }

            if (!IsAlive)
            {
                AgonyPeriod -= timeDelta;
                if (AgonyPeriod.Ticks < 0)
                {
                    IsVisible = false;
                    _bus.Publish(new EnemyDespawned(this));
                }
                return;
            }

            if (_currentWaypoint == 0)
            {
                Point mapLocation = ActualLocationByMap(_waypoints[_currentWaypoint].X, _waypoints[_currentWaypoint].Y);
                _currentX = mapLocation.X;
                _currentY = mapLocation.Y;

                _currentWaypoint = 1;
            }
            else
            {
                if (_waypoints.Count <= _currentWaypoint)
                {
                    FoundPointG = true;
                    _bus.Publish(new EnemyReachedGoal(this));
                    return;
                }

                Point currentWaypointMap = _waypoints[_currentWaypoint];
                Point currentWaypoint = ActualLocationByMap(currentWaypointMap.X, currentWaypointMap.Y);

                // From current position to the current waypoint.
                Vector2 toWaypoint = new Vector2(currentWaypoint.X - _currentX, currentWaypoint.Y - _currentY);

                SpriteDirected.ChangeDirection(toWaypoint);

                // Distance traveled.
                float traveled = (float)timeDelta.TotalSeconds * speed;

                // From current position to the position where we should be (position delta)
                Vector2 traveledRelative = Vector2.Lerp(Vector2.Zero, toWaypoint, traveled / toWaypoint.Length());

                _currentX += traveledRelative.X;
                _currentY += traveledRelative.Y;


                if (Math.Abs(toWaypoint.X) <= 1 && Math.Abs(toWaypoint.Y) <= 1)
                {
                    _currentWaypoint++;
                }
            }

            LocationCenter = new PointF(_currentX - SpriteDirected.Sprite.Location.Width / 2,
                _currentY - SpriteDirected.Sprite.Location.Height / 2);
        }

        public override void Render(BufferedGraphics g)
        {
            if (IsVisible)
            {
                // Render the sprite. Returns the render coordiates.
                PointF renderCoords = SpriteDirected.Render(g, _graphicsTracker, LocationCenter);

                if (IsAlive)
                {
                    g.Graphics.DrawString($"{_health.Health:#.0}", _fontsAndColors.MonospaceFontSmaller,
                        _fontsAndColors.BlueBrush, renderCoords.X, renderCoords.Y - 10);
                }
                else
                {
                    g.Graphics.DrawString("Aaaaarrrghhh!", _fontsAndColors.MonospaceFontSmaller,
                        _fontsAndColors.BlackBrush, renderCoords.X - 35, renderCoords.Y - 20);
                }
            }
        }

        public float GetDistance(TowerBase towerBase)
        {
            return 10;
            //new Vector2(_currentX - towerBase.LocationCenter.X, _currentY - towerBase.LocationCenter.Y).Length();
        }

        public void Shot(TowerBase towerBase)
        {
            _health.Decrease(towerBase.Settings.Powah);
        }
    }
}