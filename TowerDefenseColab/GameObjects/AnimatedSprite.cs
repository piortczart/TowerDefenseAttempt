using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenseColab.GameMechanisms;

namespace TowerDefenseColab.GameObjects
{
    public class AnimatedSprite : GameObjectBase
    {
        public List<Animation> Animations { get; set; }
        public Animation CurrentAnimation { get; set; }
        
        protected Size FrameSize;
        private Point _location;

        public AnimatedSprite()
        {
            
        }

        public AnimatedSprite(Size frameSize, List<Animation> animations)
        {
            Animations = animations;
            FrameSize = frameSize;
        }

        public override void Render(BufferedGraphics g)
        {
            Rectangle sourceRectangle = new Rectangle(
                CurrentAnimation.CurrentFrame * FrameSize.Width,
                0,
                FrameSize.Width,
                FrameSize.Height);
            Rectangle destRectangle = new Rectangle(
                _location, 
                FrameSize);
            
            g.Graphics.DrawImage(Sprite, destRectangle, sourceRectangle, GraphicsUnit.Pixel);
        }

        public override void Init()
        {
            
        }

        public override void Update(TimeSpan timeDelta)
        {
            CurrentAnimation.Update(timeDelta);
            _location = new Point((int)(LocationCenter.X - FrameSize.Width / 2f), (int)(LocationCenter.Y - FrameSize.Height / 2f));
        }
    }
}
