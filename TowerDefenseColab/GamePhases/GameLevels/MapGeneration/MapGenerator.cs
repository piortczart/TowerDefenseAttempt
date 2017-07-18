using System;
using System.Collections.Generic;
using System.Drawing;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;
using TowerDefenseColab.PathCreation;

namespace TowerDefenseColab.GamePhases.GameLevels.MapGeneration
{
    public class GeneratedMap
    {
        public List<Point> Path { get; set; }
        public SpriteEnum[,] Map { get; set; }
    }

    public class MapGenerator
    {
        private readonly PathCreator _pathCreator;

        public MapGenerator(PathCreator pathCreator)
        {
            _pathCreator = pathCreator;
        }

        public GeneratedMap GenerateMap()
        {
            var random = new Random();

            int mapWidth = random.Next(10, 20);
            int mapHeight = random.Next(10, 15);

            bool[,] map = new bool[mapHeight, mapWidth];
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    map[i, j] = true;
                }
            }

            // Make some percentage impassable.
            int totalCount = mapWidth * mapHeight;
            int impassablePercentage = random.Next(10, 20);
            int impassableCount = (int)Math.Ceiling(totalCount * ((float)impassablePercentage / 100));
            for (int i = 0; i < impassableCount; i++)
            {
                int x = random.Next(0, map.GetLength(0) - 1);
                int y = random.Next(0, map.GetLength(1) - 1);

                map[x, y] = false;
            }

            // Generate a start and an end.
            int start = random.Next(0, mapHeight - 1);
            int end = random.Next(0, mapHeight - 1);

            Point startPoint = new Point(0, start);
            Point endPoint = new Point(mapWidth - 1, end);

            // Create the final map. (with impassable tiles)
            SpriteEnum[,] spriteMap = new SpriteEnum[mapHeight, mapWidth];
            for (int h = 0; h < spriteMap.GetLength(0); h++)
            {
                for (int w = 0; w < spriteMap.GetLength(1); w++)
                {
                    spriteMap[h, w] = map[h, w] ? SpriteEnum.LandscapeGrass : SpriteEnum.LandscapeMinerals;
                }
            }

            // Generate a path.
            List<Point> path = _pathCreator.CreatePath(map, startPoint, endPoint);
            Point previous = Point.Empty;
            for (int i = 0; i < path.Count; i++)
            {
                SpriteDirectionEnum from;
                SpriteDirectionEnum to;

                Point next = i == path.Count - 1 ? Point.Empty : path[i + 1];

                Point current = path[i];
                if (i == 0)
                {
                    from = SpriteDirectionEnum.BottomLeft;
                    if (next.X > current.X)
                    {
                        to = SpriteDirectionEnum.TopRight;
                    }
                    else if (next.Y < current.Y)
                    {
                        to = SpriteDirectionEnum.TopLeft;
                    }
                    else if (next.Y > current.Y)
                    {
                        to = SpriteDirectionEnum.BottomRight;
                    }
                    else
                    {
                        throw new Exception("That's unexpected.");
                    }
                }
                else
                {
                    if (previous.Y == current.Y)
                    {
                        // Straight.
                        from = SpriteDirectionEnum.BottomLeft;
                    }
                    else if (previous.Y > current.Y)
                    {
                        from = SpriteDirectionEnum.BottomRight;
                    }
                    else if (previous.Y < current.Y)
                    {
                        from = SpriteDirectionEnum.TopLeft;
                    }
                    else
                    {
                        throw new Exception("That's unexpected.");
                    }

                    if (i == path.Count - 1)
                    {
                        // The last one.
                        to = SpriteDirectionEnum.TopRight;
                    }
                    else
                    {
                        if (next.X > current.X)
                        {
                            to = SpriteDirectionEnum.TopRight;
                        }
                        else if (next.Y < current.Y)
                        {
                            to = SpriteDirectionEnum.TopLeft;
                        }
                        else if (next.Y > current.Y)
                        {
                            to = SpriteDirectionEnum.BottomRight;
                        }
                        else
                        {
                            throw new Exception("That's unexpected.");
                        }
                    }
                }
                previous = path[i];

                if (from == SpriteDirectionEnum.BottomLeft && to == SpriteDirectionEnum.TopRight)
                {
                    spriteMap[current.Y, current.X] = SpriteEnum.LandscapeRoadUp;
                }
                else if (from == SpriteDirectionEnum.TopLeft && to == SpriteDirectionEnum.BottomRight ||
                         from == SpriteDirectionEnum.BottomRight && to == SpriteDirectionEnum.TopLeft)
                {
                    spriteMap[current.Y, current.X] = SpriteEnum.LandscapeRoadDown;
                }
                else if (from == SpriteDirectionEnum.BottomLeft && to == SpriteDirectionEnum.BottomRight)
                {
                    spriteMap[current.Y, current.X] = SpriteEnum.LandscapeTurnBottomLeftBottomRight;
                }
                else if (from == SpriteDirectionEnum.TopLeft && to == SpriteDirectionEnum.TopRight)
                {
                    spriteMap[current.Y, current.X] = SpriteEnum.LandscapeTurnTopLeftTopRight;
                }
                else if (from == SpriteDirectionEnum.BottomLeft && to == SpriteDirectionEnum.TopLeft)
                {
                    spriteMap[current.Y, current.X] = SpriteEnum.LandscapeTurnBottomLeftTopLeft;
                }
                else if (from == SpriteDirectionEnum.BottomRight && to == SpriteDirectionEnum.TopRight)
                {
                    spriteMap[current.Y, current.X] = SpriteEnum.LandscapeTurnBottomRightTopRight;
                }
                else
                {
                    throw new Exception("That's unexpected.");
                }
            }
            return new GeneratedMap
            {
                Map = spriteMap,
                Path = path
            };
        }
    }
}