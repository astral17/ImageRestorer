using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ImageRestorer
{
    public partial class PuzzleEditor : Form
    {
        private string imagesPath, savePath, puzzleName;
        private int tileSize;
        private SolutionMixer solutions;
        private Bitmap bitmap;
        private PuzzleTile[,] tiles = new PuzzleTile[128, 128];
        private static Rectangle EmptyRectangle = new Rectangle(-1, -1, -1, -1);
        private Rectangle selected = EmptyRectangle;
        private static readonly SolidBrush selectionBrush = new SolidBrush(Color.FromArgb(64, 64, 64, 128));
        private bool isMoving = false;
        private Point position = Point.Empty;
        private void SwapTiles(int x1, int y1, int x2, int y2)
        {
            PuzzleTile tile = tiles[x1, y1];
            tiles[x1, y1] = tiles[x2, y2];
            tiles[x2, y2] = tile;
        }
        private void SolveSelected()
        {
            if (selected.Width != -1)
            {
                Puzzle puzzle = new Puzzle(selected.Width, selected.Height, tileSize);
                for (int y = selected.Top; y < selected.Bottom; y++)
                    for (int x = selected.Left; x < selected.Right; x++)
                    {
                        if (tiles[x, y] == null)
                            return;
                        puzzle.tiles[x - selected.Left, y - selected.Top] = tiles[x, y];
                    }
                Solver.BFSSolve(puzzle);
                for (int y = selected.Top; y < selected.Bottom; y++)
                    for (int x = selected.Left; x < selected.Right; x++)
                    {
                        if (tiles[x, y] == null)
                            return;
                        tiles[x, y] = puzzle.tiles[x - selected.Left, y - selected.Top];
                    }
            }
        }
        private void FullSavePuzzle()
        {
            solutions.Save(savePath);
        }
        private void SavePuzzle()
        {
            Puzzle puzzle = new Puzzle(bitmap.Width / tileSize, bitmap.Height / tileSize, tileSize);
            for (int y = 0; y < puzzle.height; y++)
            {
                for (int x = 0; x < puzzle.width; x++)
                {
                    puzzle.tiles[x, y] = tiles[x, y];
                }
            }
            solutions.Add(puzzle, puzzleName);
        }
        private Int64 GetScore()
        {
            Puzzle puzzle = new Puzzle(bitmap.Width / tileSize, bitmap.Height / tileSize, tileSize);
            for (int y = 0; y < puzzle.height; y++)
            {
                for (int x = 0; x < puzzle.width; x++)
                {
                    if (tiles[x, y] == null)
                        return Int64.MaxValue;
                    puzzle.tiles[x, y] = tiles[x, y];
                    //tiles[x, y].bitmap.Save(String.Format("tiles/tile{0} {1}.png", x, y));
                }
            }
            return puzzle.GetTotalScore();
        }
        private void RemoveSelection()
        {
            isMoving = false;
            selected = EmptyRectangle;
            Refresh();
        }
        private string ReadNewPuzzleName()
        {
            string puzzleName;
            bool first = true;
            do
            {
                if (!first)
                {
                    Console.WriteLine("Incorrect File");
                }
                first = false;
                Console.Write("Enter the Puzzle Name: ");
                puzzleName = Console.ReadLine();
            } while (!File.Exists(Path.Combine(imagesPath, puzzleName)));
            return puzzleName;
        }
        private void LoadFromImage(string puzzleName)
        {
            bitmap = new Bitmap(Image.FromFile(Path.Combine(imagesPath, puzzleName)));
            int index = 0;
            for (int y = 0; y < bitmap.Height / tileSize; y++)
            {
                for (int x = 0; x < bitmap.Width / tileSize; x++)
                {
                    tiles[x, y] = new PuzzleTile(bitmap.Clone(new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), System.Drawing.Imaging.PixelFormat.DontCare), index++);
                }
            }
        }
        private void LoadPuzzle(string puzzleName)
        {
            this.puzzleName = puzzleName;
            if (solutions.Contains(puzzleName))
            {
                bitmap = new Bitmap(Image.FromFile(Path.Combine(imagesPath, puzzleName)));
                Puzzle puzzle = new Puzzle(Path.Combine(imagesPath, puzzleName), tileSize);
                puzzle.SetPermutation(solutions.Get(puzzleName));
                tiles = new PuzzleTile[128, 128];
                for (int y = 0; y < puzzle.height; y++)
                {
                    for (int x = 0; x < puzzle.width; x++)
                    {
                        tiles[x, y] = puzzle.tiles[x, y];
                    }
                }
            }
            else
                LoadFromImage(puzzleName);
        }
        public PuzzleEditor(string imagesPath, int tileSize, string savePath, string puzzleName = null)
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.imagesPath = imagesPath;
            this.tileSize = tileSize;
            this.savePath = savePath;
            solutions = new SolutionMixer(imagesPath, tileSize, savePath);
            if (String.IsNullOrEmpty(puzzleName))
                puzzleName = ReadNewPuzzleName();
            LoadPuzzle(puzzleName);
        }
        private void PuzzleEditor_Paint(object sender, PaintEventArgs e)
        {
            //lock (e.Graphics)
            //{
            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    if (tiles[x, y] != null)
                        e.Graphics.DrawImage(tiles[x, y].bitmap, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), 0, 0, tileSize, tileSize, GraphicsUnit.Pixel);
                }
            }
            if (!isMoving && selected.Width != -1)
            {
                e.Graphics.FillRectangle(selectionBrush, new Rectangle(selected.X * tileSize, selected.Y * tileSize, selected.Width * tileSize, selected.Height * tileSize));
            }
            if (isMoving)
            {
                for (int y = selected.Top; y < selected.Bottom; y++)
                    for (int x = selected.Left; x < selected.Right; x++)
                    {
                        if (tiles[x, y] != null)
                            e.Graphics.DrawImage(tiles[x, y].bitmap, new Rectangle((x - selected.Left + position.X) * tileSize, (y - selected.Top + position.Y) * tileSize, tileSize, tileSize), 0, 0, tileSize, tileSize, GraphicsUnit.Pixel);
                    }
            }
            //Console.WriteLine("!!!");
            //}
        }

        private void PuzzleEditor_MouseDown(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                for (int y = selected.Top; y < selected.Bottom; y++)
                    for (int x = selected.Left; x < selected.Right; x++)
                    {
                        if (tiles[x, y] != null)
                            SwapTiles(x, y, x - selected.Left + position.X, y - selected.Top + position.Y);
                    }
            }
            else
            {
                selected = EmptyRectangle;
                selected.X = e.X / tileSize;
                selected.Y = e.Y / tileSize;
            }
        }

        private void PuzzleEditor_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                position = new Point(e.X / tileSize, e.Y / tileSize);
                Refresh();
            }
        }

        private void PuzzleEditor_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isMoving)
            {
                selected.Width = e.X / tileSize;
                if (selected.X > selected.Width)
                {
                    int tmp = selected.X;
                    selected.X = selected.Width;
                    selected.Width = tmp;
                }
                selected.Height = e.Y / tileSize;
                if (selected.Y > selected.Height)
                {
                    int tmp = selected.Y;
                    selected.Y = selected.Height;
                    selected.Height = tmp;
                }
                selected.Width -= selected.X - 1;
                selected.Height -= selected.Y - 1;
                Refresh();
            }
            else
                RemoveSelection();
        }

        private void PuzzleEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.M:
                    isMoving = true;
                    break;
                case Keys.S:
                    if (e.Control)
                        FullSavePuzzle();
                    else
                        SavePuzzle();
                    break;
                case Keys.D:
                    Console.WriteLine("score: {0}", GetScore());
                    break;
                case Keys.O:
                    LoadPuzzle(ReadNewPuzzleName());
                    Refresh();
                    break;
                case Keys.D1:
                    SolveSelected();
                    RemoveSelection();
                    break;
            }
        }
    }
}
