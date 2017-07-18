using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using TowerDefenseColab.GraphicsPoo;

namespace TowerDefenseColab.PathCreation
{
    public class PathCreator
    {
        public class SuperPoint
        {
            public Point Coords { get; set; }

            /// <summary>
            /// "H" score
            /// </summary>
            public float Heuristic { get; set; }

            /// <summary>
            /// How for away is the node from it's parent (?).
            /// For me always 1.
            /// </summary>
            public float Gscore { get; set; }

            public float FScore => Gscore + Heuristic;

            public SuperPoint Parent { get; set; }
            public bool IsPassable { get; set; }
        }

        public List<Point> CreatePath(bool[,] map, Point start, Point end)
        {
            SuperPoint[][] spoint = MakeSuperPoints(map, end);

            // Check open list for lowest F (if the same -> take the one added to open list later)
            // Add it to closed list (remove from open list)
            // Add all non-blocked neighbours (not open/closed) to the open list (parent is the one closed).
            // Calculate the "heuristic" for all item in the open list.
            // Calculate G-score for all in the open list: is 1 + G score of parent (G+H = F)
            // Check adjacent tiles on open list, is it better to go through currently closed
            //   - to check it just take the G score of current, add the cost of going to the one being checked and see if it's lower than what is already in the one being checked
            //   - if it's lower recalculate F for the one being checked, based on the new G score
            //   - and change the parent

            var closed = new List<SuperPoint>();
            var open = new List<SuperPoint> { spoint[start.Y][start.X] };

            while (open.Count > 0)
            {
                // Get the one with lowest F.
                SuperPoint current = open.OrderBy(e => e.FScore).First();
                // Add it to closed, remove from open.
                open.Remove(current);
                closed.Add(current);

                if (current == spoint[end.Y][end.X])
                {
                    break;
                }

                // Find all non-blocked neighbours (not in open nor closed)
                List<SuperPoint> neighbours =
                    GetNeighbours(current.Coords, spoint)
                    .Where(e => e.IsPassable && !open.Contains(e) && !closed.Contains(e))
                    .ToList();
                open.AddRange(neighbours);

                // Calculate the G score for all neighbours. Parent + path from parent to them.
                neighbours.ForEach(n => n.Gscore = current.Gscore + 1);
                neighbours.ForEach(n => n.Parent = current);

                // Find the neighbours which are still open.
                List<SuperPoint> neighboursInOpen =
                    GetNeighbours(current.Coords, spoint)
                    .Where(e => e.IsPassable && open.Contains(e))
                    .ToList();

                foreach (SuperPoint neighbourOpen in neighboursInOpen)
                {
                    // Calculate the potential new cost.
                    float potentialG = current.Gscore + 1;
                    if (potentialG < neighbourOpen.Gscore)
                    {
                        neighbourOpen.Gscore = potentialG;
                        neighbourOpen.Parent = current;
                    }
                }
            }

            var superResult = new List<SuperPoint>
            {
                spoint[end.Y][end.X]
            };
            while (true)
            {
                SuperPoint current = superResult[superResult.Count - 1];
                if (current.Parent == null)
                {
                    break;
                }
                superResult.Add(current.Parent);

                if (superResult.Count > map.GetLength(0) * map.GetLength(1))
                {
                    throw new Exception("Invalid path. Circles or something.");
                }
            }

            var result = superResult.Select(sp => sp.Coords).ToList();
            result.Reverse();
            return result;
        }

        public SuperPoint[][] MakeSuperPoints(bool[,] map, Point end)
        {
            SuperPoint[][] spoint = new SuperPoint[map.GetLength(0)][];
            // Make measurements from each point to the end.
            // This is a simple type of heuristic calculation.
            for (int y = 0; y < map.GetLength(0); y++)
            {
                spoint[y] = new SuperPoint[map.GetLength(1)];
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    var coords = new Point(x, y);
                    spoint[y][x] = new SuperPoint
                    {
                        Coords = coords,
                        Heuristic = Vector2.Distance(coords.ToVector2(), end.ToVector2()),
                        IsPassable = map[y, x]
                    };
                }
            }
            return spoint;
        }

        public IEnumerable<SuperPoint> GetNeighbours(Point point, SuperPoint[][] spoint)
        {
            if (point.X > 0)
            {
                // The one to the left.
                yield return spoint[point.Y][point.X - 1];
            }
            if (point.Y > 0)
            {
                // The one above.
                yield return spoint[point.Y - 1][point.X];
            }
            if (point.X < spoint[0].Length - 1)
            {
                // The one to the right.
                yield return spoint[point.Y][point.X + 1];
            }
            if (point.Y < spoint.Length - 1)
            {
                // The one to the bottom.
                yield return spoint[point.Y + 1][point.X];
            }
        }
    }
}