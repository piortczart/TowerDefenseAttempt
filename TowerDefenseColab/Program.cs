using System;
using System.Windows.Forms;
using StructureMap;
using TowerDefenseColab.GameBusHere;
using TowerDefenseColab.GameObjects;
using TowerDefenseColab.GamePhases;
using TowerDefenseColab.GamePhases.GameLevels;
using TowerDefenseColab.Logging;

namespace TowerDefenseColab
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Container ioc = GetIoC();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (GameWindow gw = ioc.GetInstance<GameWindow>())
            {
                gw.GameLoop();
            }
        }

        private static Container GetIoC()
        {
            return new Container(_ =>
            {
                _.ForSingletonOf<GamePhaseManager>();
                _.ForSingletonOf<InputManager>();
                _.ForSingletonOf<GameLevelFactory>();
                _.ForSingletonOf<EnemyFactory>();
                _.ForSingletonOf<GraphicsTracker>();
                _.ForSingletonOf<GameBus>();
                _.ForSingletonOf<MouseDragControl>();
                _
                .ForSingletonOf<ApplicationLogger>()
                .Use<ApplicationLogger>()
                .Ctor<Action<string>>("action")
                .Is(x => { });
            });
        }
    }
}