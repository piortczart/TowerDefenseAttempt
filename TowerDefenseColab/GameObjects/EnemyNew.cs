using System;
using System.Drawing;
using TowerDefenseColab.GraphicsPoo;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;

namespace TowerDefenseColab.GameObjects
{
    public class EnemyNew : GameLoopMethods
    {
        private readonly SpriteDetails _spriteDetails;
        private readonly GraphicsTracker _graphicsTracker;

        private float _x, _y;

        public EnemyNew(SpriteDetails spriteDetails, GraphicsTracker graphicsTracker)
        {
            _spriteDetails = spriteDetails;
            _graphicsTracker = graphicsTracker;
        }

        public override void Init()
        {
        }

        public override void Update(TimeSpan timeDelta)
        {
            _x += (float)timeDelta.TotalSeconds * 10;
            _y += (float)timeDelta.TotalSeconds * 5;
        }

        public override void Render(BufferedGraphics g)
        {
            g.Graphics.DrawImage(_spriteDetails.Bitmap, _graphicsTracker.DisplayOffset.X + _x, _graphicsTracker.DisplayOffset.Y + _y);
        }
    }
}