using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerDefenseColab.GamePhases.GameLevels;

namespace TowerDefenseColab.Tests
{
    [TestClass]
    public class GameLevelActionLimiterTests
    {
        [TestMethod]
        public void GameLevelActionLimiter_TwoRequests_TimeNotStarted()
        {
            var time = new GameLevelTime();
            var limiter = new GameLevelActionLimiter(time, TimeSpan.FromSeconds(1));

            Assert.IsTrue(limiter.CanDoStuff());
            limiter.DoingIt();

            Assert.IsFalse(limiter.CanDoStuff());
            limiter.DoingIt();
        }

        [TestMethod]
        public void GameLevelActionLimiter_TwoRequestsTooFast_TimeStarted()
        {
            var time = new GameLevelTime();
            var limiter = new GameLevelActionLimiter(time, TimeSpan.FromSeconds(10));

            time.Start();
            Assert.IsTrue(limiter.CanDoStuff());
            limiter.DoingIt();

            Assert.IsFalse(limiter.CanDoStuff());
            limiter.DoingIt();
        }

        [TestMethod]
        public async Task GameLevelActionLimiter_TwoRequests_TimeStarted()
        {
            var time = new GameLevelTime();
            var limiter = new GameLevelActionLimiter(time, TimeSpan.FromMilliseconds(100));

            time.Start();
            Assert.IsTrue(limiter.CanDoStuff());
            limiter.DoingIt();

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            Assert.IsTrue(limiter.CanDoStuff());
            limiter.DoingIt();
        }
    }
}