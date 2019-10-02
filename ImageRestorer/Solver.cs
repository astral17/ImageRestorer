using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ImageRestorer
{
    public static class Solver
    {
        private class TilePointer
        {
            public int x, y;
            public PuzzleTile tile;
            public bool used = false;
            private Puzzle puzzle;
            public int size => tile.size;
            public Bitmap bitmap => tile.bitmap;
            public TilePointer(Puzzle puzzle, int x, int y)
            {
                this.puzzle = puzzle;
                this.x = x;
                this.y = y;
                tile = puzzle.tiles[x, y];
            }
            public void SwapWith(TilePointer other)
            {
                puzzle.Swap(x, y, other.x, other.y);
                int tmp = x;
                x = other.x;
                other.x = tmp;

                tmp = y;
                y = other.y;
                other.y = tmp;
            }
        }
        private static Int64 GetColorDistance(Color color1, Color color2)
        {
            //return (Int64)(Math.Pow(Math.Abs(color1.R - color2.R), 2.0) + Math.Pow(Math.Abs(color1.G - color2.G), 2.0) + Math.Pow(Math.Abs(color1.B - color2.B), 2.0));
            return Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
        }
        public static void NaiveSolve(Puzzle puzzle)
        {
            List<TilePointer> tiles = new List<TilePointer>();

            for (int y = 0; y < puzzle.height; y++)
            {
                for (int x = 0; x < puzzle.width; x++)
                {
                    tiles.Add(new TilePointer(puzzle, x, y));
                }
            }

            //Directory.CreateDirectory("results");
            
            for (int y = 0; y < puzzle.height; y++)
            {
                for (int x = 0; x < puzzle.width; x++)
                {
                    if (x == 0 && y == 0)
                    {
                        tiles[0].used = true;
                        continue;
                    }
                    TilePointer best = null;
                    Int64 bestScore = Int64.MaxValue;
                    foreach (TilePointer tile in tiles)
                    {
                        if (!tile.used)
                        {
                            Int64 score = 0;
                            if (x == 0)
                            {
                                for (int i = 0; i < puzzle.tileSize; i++)
                                    score += GetColorDistance(puzzle.tiles[x, y - 1].bitmap.GetPixel(i, puzzle.tileSize - 1), tile.tile.bitmap.GetPixel(i, 0));
                            }
                            else
                            {
                                for (int i = 0; i < puzzle.tileSize; i++)
                                    score += GetColorDistance(puzzle.tiles[x - 1, y].bitmap.GetPixel(puzzle.tileSize - 1, i), tile.tile.bitmap.GetPixel(0, i));
                            }

                            if (score < bestScore)
                            {
                                best = tile;
                                bestScore = score;
                            }
                        }
                    }
                    if (best == null)
                    {
                        throw new Exception("tile not found");
                    }
                    best.used = true;
                    foreach (TilePointer tile in tiles)
                    {
                        if (tile.x == x && tile.y == y)
                        {
                            tile.SwapWith(best);
                            break;
                        }
                    }
                    //puzzle.Save(String.Format("results/{0} {1}.png", y, x));
                }
            }

        }

        //
        private enum ConnectDirection
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3,
        }
        private static readonly Random random = new Random();
        private class TwoTilesEdge : IComparable
        {
            public ConnectDirection direction;
            public AdvancedTile tile1, tile2;
            public Int64 score;
            public int id = random.Next();
            public TwoTilesEdge(ConnectDirection direction, AdvancedTile tile1, AdvancedTile tile2)
            {
                score = 0;
                switch (direction)
                {
                    case ConnectDirection.Right:
                        for (int i = 0; i < tile1.size; i++)
                            score += GetColorDistance(tile1.bitmap.GetPixel(tile1.size - 1, i), tile2.bitmap.GetPixel(0, i));
                        break;
                    case ConnectDirection.Left:
                        for (int i = 0; i < tile1.size; i++)
                            score += GetColorDistance(tile1.bitmap.GetPixel(0, i), tile2.bitmap.GetPixel(tile1.size - 1, i));
                        break;
                    case ConnectDirection.Bottom:
                        for (int i = 0; i < tile1.size; i++)
                            score += GetColorDistance(tile1.bitmap.GetPixel(i, tile1.size - 1), tile2.bitmap.GetPixel(i, 0));
                        break;
                    case ConnectDirection.Top:
                        for (int i = 0; i < tile1.size; i++)
                            score += GetColorDistance(tile1.bitmap.GetPixel(i, 0), tile2.bitmap.GetPixel(i, tile1.size - 1));
                        break;
                }

                this.direction = direction;
                this.tile1 = tile1;
                this.tile2 = tile2;
            }

            public int CompareTo(object obj)
            {
                if (score.Equals((obj as TwoTilesEdge).score))
                {
                    if (((int)direction).Equals((int)(obj as TwoTilesEdge).direction))
                        return id.CompareTo((obj as TwoTilesEdge).id);
                    return ((int)direction).CompareTo((int)(obj as TwoTilesEdge).direction);
                }
                return score.CompareTo((obj as TwoTilesEdge).score);
            }
        }
        private enum TileState
        {
            Open,
            Selected,
            Closed,
        }
        private class AdvancedTile : PuzzleTile
        {
            public TileState state = TileState.Open;
            public int x = int.MaxValue, y = int.MaxValue, free = 4;
            //public AdvancedTile[] neighbors = new AdvancedTile[4];
            //public bool[] neighbors = new bool[4];
            public AdvancedTile(PuzzleTile puzzleTile) : base(puzzleTile.bitmap, puzzleTile.index)
            {

            }
        }
        private static void AddEdges(SortedSet<TwoTilesEdge> edges, List<AdvancedTile> tiles, AdvancedTile addTile)
        {
            if (addTile.state != TileState.Open)
            {
                throw new Exception("This tile already added");
            }
            addTile.state = TileState.Selected;
            foreach (AdvancedTile tile in tiles)
            {
                if (tile.state == TileState.Open)
                {
                    edges.Add(new TwoTilesEdge(ConnectDirection.Bottom, addTile, tile));
                    edges.Add(new TwoTilesEdge(ConnectDirection.Top, addTile, tile));
                    edges.Add(new TwoTilesEdge(ConnectDirection.Left, addTile, tile));
                    edges.Add(new TwoTilesEdge(ConnectDirection.Right, addTile, tile));
                }
            }
        }
        private static void GetOffset(ConnectDirection direction, out int x, out int y)
        {
            x = y = 0;
            switch (direction)
            {
                case ConnectDirection.Left:
                    x = -1;
                    break;
                case ConnectDirection.Right:
                    x = 1;
                    break;
                case ConnectDirection.Top:
                    y = -1;
                    break;
                case ConnectDirection.Bottom:
                    y = 1;
                    break;
            }
        }
        private static ConnectDirection GetOpposite(ConnectDirection direction)
        {
            switch (direction)
            {
                case ConnectDirection.Left:
                    return ConnectDirection.Right;
                case ConnectDirection.Right:
                    return ConnectDirection.Left;
                case ConnectDirection.Top:
                    return ConnectDirection.Bottom;
                case ConnectDirection.Bottom:
                    return ConnectDirection.Top;
            }
            throw new Exception("Incorrect direction");
        }
        public static void Solve(Puzzle puzzle)
        {
            List<AdvancedTile> tiles = new List<AdvancedTile>();

            for (int y = 0; y < puzzle.height; y++)
            {
                for (int x = 0; x < puzzle.width; x++)
                {
                    tiles.Add(new AdvancedTile(puzzle.tiles[x, y]));
                }
            }
            SortedSet<TwoTilesEdge> edges = new SortedSet<TwoTilesEdge>();
            int minX = 0, minY = 0, maxX = 0, maxY = 0;
            tiles[0].x = tiles[0].y = 0;
            AddEdges(edges, tiles, tiles[0]);
            //Directory.CreateDirectory("results3");
            //int iteration = 0;
            HashSet<Point> used = new HashSet<Point>();
            used.Add(new Point(0, 0));
            while (edges.Count > 0)
            {
                // Find Best Edge
                TwoTilesEdge best = edges.Min;
                edges.Remove(best);
                // Check Statement for Edge
                if (best.tile1.state != TileState.Selected || best.tile2.state != TileState.Open)
                    continue;
                // Has place to insert
                //if (best.tile1.neighbors[(int)best.direction])
                GetOffset(best.direction, out int dx, out int dy);
                if (used.Contains(new Point(best.tile1.x + dx, best.tile1.y + dy)))
                    continue;
                // Check height width after adding
                if (maxY - minY + 1 >= puzzle.height)
                {
                    if (best.direction == ConnectDirection.Bottom && best.tile1.y == maxY)
                        continue;
                    if (best.direction == ConnectDirection.Top && best.tile1.y == minY)
                        continue;
                }

                if (maxX - minX + 1 >= puzzle.width)
                {
                    if (best.direction == ConnectDirection.Right && best.tile1.x == maxX)
                        continue;
                    if (best.direction == ConnectDirection.Left && best.tile1.x == minX)
                        continue;
                }
                // If good Update Tile state and Add new Edges
                best.tile2.x = best.tile1.x + dx;
                best.tile2.y = best.tile1.y + dy;
                minX = Math.Min(minX, best.tile2.x);
                maxX = Math.Max(maxX, best.tile2.x);
                minY = Math.Min(minY, best.tile2.y);
                maxY = Math.Max(maxY, best.tile2.y);
                //best.tile1.neighbors[(int)best.direction] = true;
                //best.tile2.neighbors[(int)GetOpposite(best.direction)] = true;
                used.Add(new Point(best.tile2.x, best.tile2.y));
                best.tile1.free--;
                best.tile2.free--;
                if (best.tile1.free == 0)
                    best.tile1.state = TileState.Closed;
                AddEdges(edges, tiles, best.tile2);
                /*/ DEBUG ONLY
                Puzzle debugPuzzle = new Puzzle(puzzle.width, puzzle.height, puzzle.tileSize);
                foreach (AdvancedTile tile in tiles)
                {
                    if (tile.state != TileState.Open)
                        debugPuzzle.tiles[tile.x - minX, tile.y - minY] = tile;
                }
                debugPuzzle.Save(String.Format("results3/{0}.png", ++iteration));//*/
            }

            foreach (AdvancedTile tile in tiles)
            {
                if (tile.state == TileState.Open)
                {
                    Console.WriteLine("ERROR: {0}", tile.index);
                    continue;
                }
                puzzle.tiles[tile.x - minX, tile.y - minY] = tile;
            }
        }
    }
}
