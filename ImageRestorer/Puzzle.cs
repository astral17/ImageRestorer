using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ImageRestorer
{
    public class PuzzleTile
    {
        public Bitmap bitmap;
        public int size, index;
        public PuzzleTile(Bitmap bitmap, int index)
        {
            this.bitmap = bitmap;
            size = bitmap.Width;
            this.index = index;
        }

    }
    public class Puzzle
    {
        public int tileSize, width, height;
        private Bitmap bitmap;
        public PuzzleTile[,] tiles;
        //public int[,] positions;
        public Puzzle(int width, int height, int tileSize)
        {
            this.width = width;
            this.height = height;
            this.tileSize = tileSize;
            bitmap = new Bitmap(width * tileSize, height * tileSize);
            tiles = new PuzzleTile[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y] = new PuzzleTile(new Bitmap(tileSize, tileSize), 0);
                }
            }
        }//*/
        public Puzzle(string fileName, int tileSize)
        {
            this.tileSize = tileSize;
            bitmap = new Bitmap(Image.FromFile(fileName));
            width = bitmap.Width / tileSize;
            height = bitmap.Height / tileSize;
            tiles = new PuzzleTile[width, height];
            //positions = new int[width, height];
            //Directory.CreateDirectory("tiles");
            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //positions[x, y] = index++;
                    tiles[x, y] = new PuzzleTile(bitmap.Clone(new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), System.Drawing.Imaging.PixelFormat.DontCare), index++);
                    //tiles[x, y].bitmap.Save(String.Format("tiles/tile{0} {1}.png", x, y));
                }
            }

        }
        public void Swap(int x1, int y1, int x2, int y2)
        {
            PuzzleTile tile = tiles[x1, y1];
            tiles[x1, y1] = tiles[x2, y2];
            tiles[x2, y2] = tile;
        }
        public void Save(string path)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                for (int y = 0; y * tileSize < bitmap.Height; y++)
                {
                    for (int x = 0; x * tileSize < bitmap.Width; x++)
                    {
                        graphics.DrawImage(tiles[x, y].bitmap, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), 0, 0, tileSize, tileSize, GraphicsUnit.Pixel);
                    }
                }
            }
            bitmap.Save(path);
        }
        public void SetPermutation(int[] permutation)
        {
            PuzzleTile[] tilesArray = new PuzzleTile[height * width];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tilesArray[tiles[x, y].index] = tiles[x, y];
                }
            }

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y] = tilesArray[permutation[index++]];
                }
            }
        }
        public void SetPermutation(string fileName)
        {
            int[] permutation = null;
            using (StreamReader reader = new StreamReader(fileName))
            {
                string[] raws = reader.ReadLine().Trim().Split(' ');
                permutation = new int[raws.Length];
                for (int i = 0; i < raws.Length; i++)
                    permutation[i] = int.Parse(raws[i]);
            }
            SetPermutation(permutation);
        }
        public string GetPermutationString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    builder.AppendFormat("{0} ", tiles[j, i].index);
            return builder.ToString();
        }
        public int[] GetPermutation()
        {
            List<int> permutation = new List<int>(width * height);
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    permutation.Add(tiles[j, i].index);
            return permutation.ToArray();
        }
        public Int64 GetTotalScore()
        {
            Int64 score = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x > 0)
                    {
                        for (int i = 0; i < tileSize; i++)
                            score += Utils.GetColorDistance(tiles[x - 1, y].bitmap.GetPixel(tileSize - 1, i), tiles[x, y].bitmap.GetPixel(0, i));
                    }
                    if (y > 0)
                    {
                        for (int i = 0; i < tileSize; i++)
                            score += Utils.GetColorDistance(tiles[x, y - 1].bitmap.GetPixel(i, tileSize - 1), tiles[x, y].bitmap.GetPixel(i, 0));
                    }
                }
            }
            return score;
        }
    }
}
