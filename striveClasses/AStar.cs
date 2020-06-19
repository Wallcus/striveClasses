using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace striveClasses
{
    //A*
    class Location
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public Location Parent;
    }

    class AStar
    {
        public static int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

        public static List<Location> GetWalkableAdjacentSquares(int x, int y, MapTile[,,] map, Enemy enemy)
        {
            var proposedLocations = new List<Location>()
        {
            new Location { X = x, Y = y - 1 },
            new Location { X = x, Y = y + 1 },
            new Location { X = x - 1, Y = y },
            new Location { X = x + 1, Y = y },
        };

            return proposedLocations.Where(l => map[l.X, l.Y, 1] == null || (map[l.X, l.Y, 1] == enemy)).ToList();
        }


        public static int[,] getPathToPlayer(GameRun run, Enemy enemy)
        {
            Location current = null; //temp location for A*
            var target = new Location { X = enemy.X, Y = enemy.Y }; //active enemy location
            var start = new Location { X = run.Player.X, Y = run.Player.Y }; //player location
            var openList = new List<Location>();
            var closedList = new List<Location>();
            int g = 0;

            //start by adding the original position to the open list
            openList.Add(start);

            while (openList.Count > 0)
            {
                //algorithm's logic

                //get the square with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                //add the current square to the closed list
                closedList.Add(current);

                //remove it from the open list
                openList.Remove(current);

                //if we added the destination to the closed list, we've found a path
                if (closedList.FirstOrDefault(l => l.Y == target.Y && l.X == target.X) != null)
                    break;

                var adjacentSquares = AStar.GetWalkableAdjacentSquares(current.X, current.Y, run.Map.Map, enemy);
                g++;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    //if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X && l.Y == adjacentSquare.Y) != null)
                        continue;

                    //if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X && l.Y == adjacentSquare.Y) == null)
                    {
                        //compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = AStar.ComputeHScore(adjacentSquare.X,
                            adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        //test if using the current G score makes the adjacent square's F score
                        //lower, if yes update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }

            int[,] path;

            if(closedList.Count != g)
            {
                //skip position 0, as it is the sentient position
                current = current.Parent;

                //-1 for skipped start and -1 for end position of target
                int pathLength = closedList.Count - 2;

                if (enemy.Speed < pathLength)
                    pathLength = enemy.Speed;

                int tempX = enemy.X;
                int tempY = enemy.Y;

                path = new int[pathLength, 2];

                for (int i = 0; i < pathLength; i++)
                {
                    path[i, 0] = current.X - tempX;
                    path[i, 1] = current.Y - tempY;
                    current = current.Parent;

                    tempX += path[i, 0];
                    tempY += path[i, 1];
                }
            }
            else
            {
                path = new int[2, 2];

                path[0, 0] = 0;
                path[0, 1] = 0;
                path[1, 0] = 0;
                path[1, 1] = 0;
            }
            return path;
        }
    }
}