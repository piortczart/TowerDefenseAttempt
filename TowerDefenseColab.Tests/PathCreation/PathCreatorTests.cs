using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerDefenseColab.PathCreation;

namespace TowerDefenseColab.Tests.PathCreation
{
    [TestClass]
    public class PathCreatorTests
    {
        [TestMethod]
        public void CreatePathTest()
        {
            var creator = new PathCreator();

            var map = new[,]
            {
                { true, true, true },
                { true, true, true },
                { false, false, true },
                { true, true, true }
            };

            List<Point> path = creator.CreatePath(map, new Point(0, 0), new Point(1, 3));

            Assert.AreEqual(7, path.Count);
            Assert.AreEqual(new Point(0, 0), path[0]);
            Assert.AreEqual(new Point(0, 1), path[1]);
            Assert.AreEqual(new Point(1, 1), path[2]);
            Assert.AreEqual(new Point(2, 1), path[3]);
            Assert.AreEqual(new Point(2, 2), path[4]);
            Assert.AreEqual(new Point(2, 3), path[5]);
            Assert.AreEqual(new Point(1, 3), path[6]);
        }

        [TestMethod]
        public void MakeSuperPointsTest()
        {
            var creator = new PathCreator();

            var map = new[,]
            {
                { true, true, true },
                { true, true, false }
            };

            var super = creator.MakeSuperPoints(map, new Point(0, 0));

            Assert.AreEqual(2, super.Length);
            Assert.AreEqual(3, super[0].Length);

            Assert.IsFalse(super[1][2].IsPassable);
        }

        [TestMethod]
        public void GetNeighboursTest()
        {
            var creator = new PathCreator();

            var map = new[,]
            {
                { true, true, false },
                { false, true, true }
            };

            var neighbours = creator.GetNeighbours(new Point(1, 1), creator.MakeSuperPoints(map, new Point(0, 0))).ToList();
            Assert.AreEqual(3, neighbours.Count);
            Assert.AreEqual(new Point(0, 1), neighbours[0].Coords);
            Assert.AreEqual(new Point(1, 0), neighbours[1].Coords);
            Assert.AreEqual(new Point(2, 1), neighbours[2].Coords);
        }
    }
}