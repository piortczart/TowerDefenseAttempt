using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenseColab.GameMechanisms;

namespace TowerDefenseColab.GameObjects
{
    public class AnimationInfo
    {
        public Size FrameSize { get; set; }

        public List<Animation> AnimationsList { get; set; }
    }
}
