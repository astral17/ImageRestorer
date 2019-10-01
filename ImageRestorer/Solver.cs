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
            return (Int64)(Math.Pow(Math.Abs(color1.R - color2.R), 2.0) + Math.Pow(Math.Abs(color1.G - color2.G), 2.0) + Math.Pow(Math.Abs(color1.B - color2.B), 2.0));
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
        private class TwoTilesEdge : IComparable
        {
            public ConnectDirection direction;
            public TilePointer tile1, tile2;
            public Int64 score;
            public TwoTilesEdge(ConnectDirection direction, TilePointer tile1, TilePointer tile2)
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
                return score.CompareTo(((TwoTilesEdge)obj).score);
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
            public int x = int.MaxValue, y = int.MaxValue;
            public AdvancedTile(PuzzleTile puzzleTile) : base(puzzleTile.bitmap, puzzleTile.index)
            {

            }
        }
        private static void UpdateState(List<PuzzleTile> tiles, List<TileState> states)
        {

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

            

        }
    }
}
