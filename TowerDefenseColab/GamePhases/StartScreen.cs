using System;
using System.Drawing;
using System.Windows.Forms;

namespace TowerDefenseColab.GamePhases
{
    public class StartScreen : GamePhase
    {
        private readonly GamePhaseManager _gamePhaseManager;
        private readonly Image _background;

        public StartScreen(InputManager inputManager, GamePhaseManager gamePhaseManager)
        {
            _gamePhaseManager = gamePhaseManager;
            inputManager.OnClick += InputManagerOnOnClick;
            _background = Image.FromFile(@"Assets\menu.png");
        }

        private void InputManagerOnOnClick(MouseEventArgs mouseEventArgs)
        {
            // Only respond to input when visible.
            if (IsVisible)
            {
                _gamePhaseManager.ChangeActiveGamePhase(GamePhaseEnum.Level001);
            }
        }

        public override void Init()
        {
        }

        public override void Update(TimeSpan timeDelta)
        {
        }

        public override void Render(BufferedGraphics g)
        {
            g.Graphics.DrawImage(_background, 0, 0);
        }
    }
}