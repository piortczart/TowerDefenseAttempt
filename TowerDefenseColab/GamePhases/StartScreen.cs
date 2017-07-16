using System;
using System.Drawing;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameBusHere.Messages;
using TowerDefenseColab.GraphicsPoo;

namespace TowerDefenseColab.GamePhases
{
    public class StartScreen : GamePhase
    {
        private readonly Image _background;
        private GraphicsTracker _graphicsTracker;

        public StartScreen(GamePhaseManager gamePhaseManager, GameBus bus, GraphicsTracker graphicsTracker)
        {
            _graphicsTracker = graphicsTracker;
            _background = Image.FromFile(@"Assets\menu.png");

            bus.Subscribe<MouseClicked>(e =>
            {
                if (IsVisible)
                {
                    gamePhaseManager.ChangeActiveGamePhase(GamePhaseEnum.Level001);
                }
            });
        }

        public override void Init()
        {
        }

        public override void Update(TimeSpan timeDelta)
        {
        }

        public override void Render(BufferedGraphics g)
        {
            g.Graphics.FillRectangle(Brushes.Gray, _graphicsTracker.DisplayRectangle);
            g.Graphics.DrawImage(_background, 0, 0);
        }
    }
}