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
                                    score += Utils.GetColorDistance(puzzle.tiles[x, y - 1].bitmap.GetPixel(i, puzzle.tileSize - 1), tile.tile.bitmap.GetPixel(i, 0));
                            }
                            else
                            {
                                for (int i = 0; i < puzzle.tileSize; i++)
                                    score += Utils.GetColorDistance(puzzle.tiles[x - 1, y].bitmap.GetPixel(puzzle.tileSize - 1, i), tile.tile.bitmap.GetPixel(0, i));
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
        private static Int64 GetScoreBetween(ConnectDirection direction, Bitmap tile1, Bitmap tile2)
        {
            Int64 score = 0;
            int size = tile1.Width;
            switch (direction)
            {
                case ConnectDirection.Right:
                    for (int i = 0; i < size; i++)
                        score += Utils.GetColorDistance(tile1.GetPixel(size - 1, i), tile2.GetPixel(0, i));
                    break;
                case ConnectDirection.Left:
                    for (int i = 0; i < size; i++)
                        score += Utils.GetColorDistance(tile1.GetPixel(0, i), tile2.GetPixel(size - 1, i));
                    break;
                case ConnectDirection.Bottom:
                    for (int i = 0; i < size; i++)
                        score += Utils.GetColorDistance(tile1.GetPixel(i, size - 1), tile2.GetPixel(i, 0));
                    break;
                case ConnectDirection.Top:
                    for (int i = 0; i < size; i++)
                        score += Utils.GetColorDistance(tile1.GetPixel(i, 0), tile2.GetPixel(i, size - 1));
                    break;
            }
            return score;
        }
        private class TwoTilesEdge : IComparable
        {
            public ConnectDirection direction;
            public AdvancedTile tile1, tile2;
            public Int64 score;
            public int id ;
            public TwoTilesEdge(ConnectDirection direction, AdvancedTile tile1, AdvancedTile tile2, int index)
            {
                id = index;
                score = 0;
                switch (direction)
                {
                    case ConnectDirection.Right:
                        for (int i = 0; i < tile1.size; i++)
                            score += Utils.GetColorDistance(tile1.bitmap.GetPixel(tile1.size - 1, i), tile2.bitmap.GetPixel(0, i));
                        break;
                    case ConnectDirection.Left:
                        for (int i = 0; i < tile1.size; i++)
                            score += Utils.GetColorDistance(tile1.bitmap.GetPixel(0, i), tile2.bitmap.GetPixel(tile1.size - 1, i));
                        break;
                    case ConnectDirection.Bottom:
                        for (int i = 0; i < tile1.size; i++)
                            score += Utils.GetColorDistance(tile1.bitmap.GetPixel(i, tile1.size - 1), tile2.bitmap.GetPixel(i, 0));
                        break;
                    case ConnectDirection.Top:
                        for (int i = 0; i < tile1.size; i++)
                            score += Utils.GetColorDistance(tile1.bitmap.GetPixel(i, 0), tile2.bitmap.GetPixel(i, tile1.size - 1));
                        break;
                }

                this.direction = direction;
                this.tile1 = tile1;
                this.tile2 = tile2;
            }

            public int CompareTo(object obj)
            {
                if (score.Equals((obj as TwoTilesEdge).score))
                    return id.CompareTo((obj as TwoTilesEdge).id);
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
            //public TileState state = TileState.Open;
            public bool selected = false;
            public int x = int.MaxValue, y = int.MaxValue;
            //public AdvancedTile[] neighbors = new AdvancedTile[4];
            //public bool[] neighbors = new bool[4];
            public AdvancedTile(PuzzleTile puzzleTile) : base(puzzleTile.bitmap, puzzleTile.index)
            {

            }
        }
        private static void AddEdges(SortedSet<TwoTilesEdge> edges, List<AdvancedTile> tiles, AdvancedTile addTile, ref int index)
        {
            //if (addTile.state != TileState.Open)
            if (addTile.selected)
            {
                throw new Exception("This tile already added");
            }
            //addTile.state = TileState.Selected;
            addTile.selected = true;
            foreach (AdvancedTile tile in tiles)
            {
                //if (tile.state == TileState.Open)
                if (!tile.selected)
                {
                    edges.Add(new TwoTilesEdge(ConnectDirection.Bottom, addTile, tile, ++index));
                    edges.Add(new TwoTilesEdge(ConnectDirection.Top, addTile, tile, ++index));
                    edges.Add(new TwoTilesEdge(ConnectDirection.Left, addTile, tile, ++index));
                    edges.Add(new TwoTilesEdge(ConnectDirection.Right, addTile, tile, ++index));
                }
            }
        }
        private static readonly int[] directionOffsetX = new int[4]
        {
            -1, 1, 0, 0,
        };
        private static readonly int[] directionOffsetY = new int[4]
        {
            0, 0, -1, 1,
        };
        private static void GetOffset(ConnectDirection direction, out int x, out int y)
        {
            x = directionOffsetX[(int)direction];
            y = directionOffsetY[(int)direction];
            /*x = y = 0;
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
            }*/
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
        public static void BFSSolve(Puzzle puzzle)
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
            int uindex = 0;
            AddEdges(edges, tiles, tiles[0], ref uindex);
            /*/
            Directory.CreateDirectory("results3418");
            int iteration = 0;//*/
            HashSet<Point> used = new HashSet<Point>();
            used.Add(new Point(0, 0));
            while (edges.Count > 0)
            {
                // Find Best Edge
                TwoTilesEdge best = edges.Min;
                edges.Remove(best);
                // Check Statement for Edge
                //if (best.tile1.state != TileState.Selected || best.tile2.state != TileState.Open)
                if (!best.tile1.selected || best.tile2.selected)
                    continue;
                //if (best.tile1.neighbors[(int)best.direction])
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
                // Has place to insert
                GetOffset(best.direction, out int dx, out int dy);
                if (used.Contains(new Point(best.tile1.x + dx, best.tile1.y + dy)))
                    continue;
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
                /*best.tile1.free--;
                best.tile2.free--;
                if (best.tile1.free == 0)
                    best.tile1.state = TileState.Closed;*/
                AddEdges(edges, tiles, best.tile2, ref uindex);
                /*/ DEBUG ONLY
                Puzzle debugPuzzle = new Puzzle(puzzle.width, puzzle.height, puzzle.tileSize);
                foreach (AdvancedTile tile in tiles)
                {
                    if (tile.x != int.MaxValue)
                        debugPuzzle.tiles[tile.x - minX, tile.y - minY] = tile;
                }
                debugPuzzle.Save(String.Format("results3418/{0}.png", ++iteration));//*/
            }
            /*
            if (used.Count < puzzle.tileSize * puzzle.tileSize)
            {
                for (int y = 0; y < puzzle.height; y++)
                    for (int x = 0; x < puzzle.width; x++)
                        puzzle.tiles[x, y] = new PuzzleTile(new Bitmap(puzzle.tileSize, puzzle.tileSize), 0);
            }*/
            foreach (AdvancedTile tile in tiles)
            {
                /*if (tile.state == TileState.Open)
                {
                    Console.WriteLine("ERROR: {0}", tile.index);
                    continue;
                }*/
                if (tile.x != int.MaxValue)
                    puzzle.tiles[tile.x - minX, tile.y - minY] = tile;
            }
            /*
            if (used.Count < puzzle.tileSize * puzzle.tileSize)
            {
                puzzle.Save("bug.png");
                throw new Exception("BUG");
            }*/
        }
        //
        // TODO: Fix bug 3512.png
        public static void VHSolve(Puzzle puzzle)
        {
            List<PuzzleTile> tiles = new List<PuzzleTile>();
            List<bool> used = new List<bool>();

            for (int y = 0; y < puzzle.height; y++)
            {
                for (int x = 0; x < puzzle.width; x++)
                {
                    tiles.Add(puzzle.tiles[x, y]);
                    used.Add(false);
                }
            }

            //PuzzleTile best = null;
            int best, index;
            bool bestFromLeftOrTop = false;
            Int64 bestScore;
            LinkedList<PuzzleTile> horizontal = new LinkedList<PuzzleTile>();
            horizontal.AddLast(tiles[0]);
            while (horizontal.Count < puzzle.width)
            {
                best = -1;
                bestScore = Int64.MaxValue;
                index = 0;
                foreach (PuzzleTile tile in tiles)
                {
                    if (used[index])
                    {
                        index++;
                        continue;
                    }
                    Int64 score = GetScoreBetween(ConnectDirection.Left, horizontal.First.Value.bitmap, tile.bitmap);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = index;
                        bestFromLeftOrTop = true;
                    }
                    score = GetScoreBetween(ConnectDirection.Right, horizontal.Last.Value.bitmap, tile.bitmap);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = index;
                        bestFromLeftOrTop = false;
                    }
                    index++;
                }
                if (best == -1)
                {
                    throw new Exception("Tile Not Found!");
                }
                if (bestFromLeftOrTop)
                    horizontal.AddFirst(tiles[best]);
                else
                    horizontal.AddLast(tiles[best]);
                used[best] = true;
            }
            /*/ DEBUG ONLY
            Puzzle debugPuzzle = new Puzzle(puzzle.width, puzzle.height, puzzle.tileSize);
            index = 0;
            foreach (PuzzleTile tile in horizontal)
            {
                debugPuzzle.tiles[index++, 0] = tile;
            }
            debugPuzzle.Save(String.Format("results3/{0}.png", "TMP1"));//*/

            //

            LinkedList<PuzzleTile> vertical = new LinkedList<PuzzleTile>();
            vertical.AddLast(horizontal.First.Value);
            while (vertical.Count < puzzle.height)
            {
                best = -1;
                bestScore = Int64.MaxValue;
                index = 0;
                foreach (PuzzleTile tile in tiles)
                {
                    if (used[index])
                    {
                        index++;
                        continue;
                    }
                    Int64 score = GetScoreBetween(ConnectDirection.Top, vertical.First.Value.bitmap, tile.bitmap);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = index;
                        bestFromLeftOrTop = true;
                    }
                    score = GetScoreBetween(ConnectDirection.Bottom, vertical.Last.Value.bitmap, tile.bitmap);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = index;
                        bestFromLeftOrTop = false;
                    }
                    index++;
                }
                if (best == -1)
                {
                    throw new Exception("Tile Not Found!");
                }
                if (bestFromLeftOrTop)
                    vertical.AddFirst(tiles[best]);
                else
                    vertical.AddLast(tiles[best]);
                used[best] = true;
            }
            /*/ DEBUG ONLY
            debugPuzzle = new Puzzle(puzzle.width, puzzle.height, puzzle.tileSize);
            int tmp_pos = 0;
            foreach (PuzzleTile tile in vertical)
            {
                if (tile == horizontal.First.Value)
                    break;
                tmp_pos++;
            }
            index = 0;
            foreach (PuzzleTile tile in vertical)
            {
                debugPuzzle.tiles[0, index++] = tile;
            }
            index = 0;
            foreach (PuzzleTile tile in horizontal)
            {
                debugPuzzle.tiles[index++, tmp_pos] = tile;
            }
            debugPuzzle.Save(String.Format("results3/{0}.png", "TMP2"));//*/

            //

            PuzzleTile[,] tilesMap = new PuzzleTile[puzzle.width, puzzle.height];
            int intersectPosition = 0;
            foreach (PuzzleTile tile in vertical)
            {
                if (tile == horizontal.First.Value)
                    break;
                intersectPosition++;
            }
            index = 0;
            foreach (PuzzleTile tile in vertical)
            {
                tilesMap[0, index++] = tile;
            }
            index = 0;
            foreach (PuzzleTile tile in horizontal)
            {
                tilesMap[index++, intersectPosition] = tile;
            }

            //CODE HERE DOWN TO UP
            for (int y = intersectPosition - 1; y >= 0; y--)
                for (int x = 1; x < puzzle.width; x++)
                {
                    best = -1;
                    bestScore = Int64.MaxValue;
                    index = 0;
                    foreach (PuzzleTile tile in tiles)
                    {
                        if (used[index])
                        {
                            index++;
                            continue;
                        }
                        //Int64 score = GetScoreBetween(ConnectDirection.Top, tilesMap[x, y + 1].bitmap, tile.bitmap) + GetScoreBetween(ConnectDirection.Right, tilesMap[x - 1, y].bitmap, tile.bitmap);
                        Int64 score = /*GetScoreBetween(ConnectDirection.Top, tilesMap[x, y + 1].bitmap, tile.bitmap) +*/ GetScoreBetween(ConnectDirection.Right, tilesMap[x - 1, y].bitmap, tile.bitmap);
                        //Int64 score = (Int64)(Math.Pow(GetScoreBetween(ConnectDirection.Top, tilesMap[x, y + 1].bitmap, tile.bitmap),1) + Math.Pow(GetScoreBetween(ConnectDirection.Right, tilesMap[x - 1, y].bitmap, tile.bitmap), 2));
                        if (score < bestScore)
                        {
                            bestScore = score;
                            best = index;
                        }
                        index++;
                    }
                    if (best == -1)
                    {
                        throw new Exception("Tile Not Found!");
                    }
                    tilesMap[x, y] = tiles[best];
                    used[best] = true;
                }

            for (int y = intersectPosition + 1; y < puzzle.height; y++)
                for (int x = 1; x < puzzle.width; x++)
                {
                    best = -1;
                    bestScore = Int64.MaxValue;
                    index = 0;
                    foreach (PuzzleTile tile in tiles)
                    {
                        if (used[index])
                        {
                            index++;
                            continue;
                        }
                        Int64 score = GetScoreBetween(ConnectDirection.Bottom, tilesMap[x, y - 1].bitmap, tile.bitmap) + GetScoreBetween(ConnectDirection.Right, tilesMap[x - 1, y].bitmap, tile.bitmap);
                        //Int64 score = /*GetScoreBetween(ConnectDirection.Bottom, tilesMap[x, y - 1].bitmap, tile.bitmap) +*/ GetScoreBetween(ConnectDirection.Right, tilesMap[x - 1, y].bitmap, tile.bitmap);
                        //Int64 score = (Int64)(Math.Pow(GetScoreBetween(ConnectDirection.Bottom, tilesMap[x, y + 1].bitmap, tile.bitmap),1) + Math.Pow(GetScoreBetween(ConnectDirection.Right, tilesMap[x - 1, y].bitmap, tile.bitmap), 2));
                        if (score < bestScore)
                        {
                            bestScore = score;
                            best = index;
                        }
                        index++;
                    }
                    if (best == -1)
                    {
                        throw new Exception("Tile Not Found!");
                    }
                    tilesMap[x, y] = tiles[best];
                    used[best] = true;
                }

            /*/ DEBUG ONLY
            debugPuzzle = new Puzzle(puzzle.width, puzzle.height, puzzle.tileSize);

            for (int y = 0; y < puzzle.height; y++)
                for (int x = 0; x < puzzle.width; x++)
                {
                    if (tilesMap[x, y] != null)
                        debugPuzzle.tiles[x, y] = tilesMap[x, y];
                }
            debugPuzzle.Save(String.Format("results3/{0}.png", "TMP3"));//*/


            for (int y = 0; y < puzzle.height; y++)
                for (int x = 0; x < puzzle.width; x++)
                    puzzle.tiles[x, y] = tilesMap[x, y];
        }

        //
        private class PuzzleLine
        {
            public LinkedList<PuzzleTile> horizontal;
            //public PuzzleLine(List<PuzzleTile> tiles, List<bool> used, int width, Puzzle puzzle, List<PuzzleLine> lines)
            public PuzzleLine(List<PuzzleTile> tiles, List<bool> used, int width)
            {
                int best, index;
                bool bestFromLeft = false;
                Int64 bestScore;
                horizontal = new LinkedList<PuzzleTile>();
                for (int i = 0; i < used.Count; i++)
                    if (!used[i])
                    {
                        horizontal.AddLast(tiles[i]);
                        used[i] = true;
                        break;
                    }
                while (horizontal.Count < width)
                {
                    best = -1;
                    bestScore = Int64.MaxValue;
                    index = 0;
                    foreach (PuzzleTile tile in tiles)
                    {
                        if (used[index])
                        {
                            index++;
                            continue;
                        }
                        Int64 score = GetScoreBetween(ConnectDirection.Left, horizontal.First.Value.bitmap, tile.bitmap);
                        if (score < bestScore)
                        {
                            bestScore = score;
                            best = index;
                            bestFromLeft = true;
                        }
                        score = GetScoreBetween(ConnectDirection.Right, horizontal.Last.Value.bitmap, tile.bitmap);
                        if (score < bestScore)
                        {
                            bestScore = score;
                            best = index;
                            bestFromLeft = false;
                        }
                        index++;
                    }
                    if (best == -1)
                    {
                        throw new Exception("Tile Not Found!");
                    }
                    if (bestFromLeft)
                        horizontal.AddFirst(tiles[best]);
                    else
                        horizontal.AddLast(tiles[best]);
                    used[best] = true;

                    /*/ DEBUG ONLY
                    Puzzle debugPuzzle = new Puzzle(width, width, puzzle.tileSize);
                    for (int y1 = 0; y1 < lines.Count; y1++)
                    {
                        index = 0;
                        foreach (PuzzleTile tile in lines[y1].horizontal)
                        {
                            debugPuzzle.tiles[index++, y1] = tile;
                        }
                    }
                    index = 0;
                    foreach (PuzzleTile tile in horizontal)
                    {
                        debugPuzzle.tiles[index++, lines.Count] = tile;
                    }
                    debugPuzzle.Save(String.Format("results4/{0} {1}.png", lines.Count, horizontal.Count));//*/
                }
            }

            public Int64 GetScoreWith(LinkedList<PuzzleTile> other, ConnectDirection direction)
            {
                LinkedListNode<PuzzleTile> node = other.First;
                Int64 score = 0;
                foreach (PuzzleTile tile in horizontal)
                {
                    score += GetScoreBetween(direction, tile.bitmap, node.Value.bitmap);
                    node = node.Next;
                }
                return score;
            }
        }
        public static void LinesSolve(Puzzle puzzle)
        {
            List<PuzzleTile> tiles = new List<PuzzleTile>();
            List<bool> used = new List<bool>();

            for (int y = 0; y < puzzle.height; y++)
            {
                for (int x = 0; x < puzzle.width; x++)
                {
                    tiles.Add(puzzle.tiles[x, y]);
                    used.Add(false);
                }
            }

            List<PuzzleLine> lines = new List<PuzzleLine>();
            //Directory.CreateDirectory("results4");
            for (int y = 0; y < puzzle.height; y++)
            {
                lines.Add(new PuzzleLine(tiles, used, puzzle.width));
                //
            }

            //

            int best, index;
            bool bestFromLeft = false;
            Int64 bestScore;
            LinkedList<PuzzleLine> vertical = new LinkedList<PuzzleLine>();
            bool[] usedLine = new bool[puzzle.height];
            vertical.AddLast(lines[0]);
            usedLine[0] = true;

            while (vertical.Count < puzzle.height)
            {
                best = -1;
                bestScore = Int64.MaxValue;
                index = 0;
                foreach (PuzzleLine line in lines)
                {
                    if (usedLine[index])
                    {
                        index++;
                        continue;
                    }
                    Int64 score = vertical.First.Value.GetScoreWith(line.horizontal, ConnectDirection.Top);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = index;
                        bestFromLeft = true;
                    }
                    score = vertical.Last.Value.GetScoreWith(line.horizontal, ConnectDirection.Bottom);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = index;
                        bestFromLeft = false;
                    }
                    index++;
                }
                if (best == -1)
                {
                    throw new Exception("Tile Not Found!");
                }
                if (bestFromLeft)
                    vertical.AddFirst(lines[best]);
                else
                    vertical.AddLast(lines[best]);
                usedLine[best] = true;

                /*/ DEBUG ONLY
                Puzzle debugPuzzle = new Puzzle(puzzle.width, puzzle.height, puzzle.tileSize);
                int indexY1 = 0;
                foreach (PuzzleLine line in vertical)
                {
                    index = 0;
                    foreach (PuzzleTile tile in line.horizontal)
                    {
                        debugPuzzle.tiles[index++, indexY1] = tile;
                    }
                    indexY1++;
                }
                debugPuzzle.Save(String.Format("results5/{0}.png", vertical.Count));//*/
            }
            //

            //

            //for (int y = 0; y < lines.Count; y++)
            int indexY = 0;
            foreach (PuzzleLine line in vertical)
            {
                index = 0;
                foreach (PuzzleTile tile in line.horizontal)
                {
                    puzzle.tiles[index++, indexY] = tile;
                }
                indexY++;
            }

        }
    }
}
