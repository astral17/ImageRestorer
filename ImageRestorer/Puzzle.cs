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
        /*public Puzzle(int width, int height, int tileSize)
        {
            this.width = width;
            this.height = height;
            this.tileSize = tileSize;
            tiles = new PuzzleTile[width, height];
            //positions = new int[width, height];
            int index = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //positions[x, y] = index++;
                    //tiles[x,y]
                }
            }
        }*/
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

            /*int position = positions[x1, y1];
            positions[x1, y1] = positions[x2, y2];
            positions[x2, y2] = position;*/
        }
        public void Save(string fileName)
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
            bitmap.Save(fileName);
        }
        public string GetPermutation()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    builder.AppendFormat("{0} ", tiles[j, i].index);
            return builder.ToString();
        }
    }
}
