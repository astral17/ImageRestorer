using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ImageRestorer
{
    class Program
    {
        static void Main(string[] args)
        {
            //*
            Puzzle puzzl2e = new Puzzle("2458.png", 64);
            //puzzl2e.Swap(3, 4, 0, 0);
            //Console.WriteLine(puzzl2e.tiles[0, 0].index);
            //Solver.NaiveSolve(puzzl2e);
            Solver.Solve(puzzl2e);
            puzzl2e.Save("result.png");
            return;//*/
            /*
            Puzzle puzzl3e = new Puzzle("0600.png", 32);
            StreamWriter sw2 = new StreamWriter("output.txt");
            Solver.Solve(puzzl3e);
            sw2.WriteLine(puzzl3e.GetPermutation());
            sw2.Close();
            return;//*/
            /*
            Puzzle puzzl2e = new Puzzle("0600.png", 32);
            StreamReader reader = new StreamReader("input.txt");
            //reader.ReadLine();
            string[] raw = reader.ReadLine().Split(' ');
            int index = 0;
            Directory.CreateDirectory("results2");
            foreach (string raw_int in raw)
            {
                Console.WriteLine(raw_int);
                int raw_pos = int.Parse(raw_int);
                int pos = -1;
                for (int i = 0; i < puzzl2e.height; i++)
                    for (int j = 0; j < puzzl2e.width; j++)
                    {
                        if (puzzl2e.tiles[i,j].index==raw_pos)
                        {
                            pos = i + j * puzzl2e.width;
                        }
                    }
                puzzl2e.Swap(index % 16, index / 16, pos % 16, pos / 16);
                puzzl2e.Save(String.Format("results2/{0}.png", index));
                index++;
            }
            puzzl2e.Save("result.png");
            StreamWriter sw = new StreamWriter("output2.txt");
            sw.WriteLine(puzzl2e.GetPermutation());
            sw.Close();
            return;//*/

            StreamWriter writer = new StreamWriter("resultA2.txt");

            foreach (string file in Directory.GetFiles("D:\\tasks\\shuffled-images-data\\data_test1_blank\\64"))
            {
                //string file = "D:\\tasks\\shuffled-images-data\\data_test1_blank\\64\\2699.png";
                Console.WriteLine(file);
                writer.WriteLine(Path.GetFileName(file));
                Puzzle puzzle = new Puzzle(file, 64);
                Solver.Solve(puzzle);
                writer.WriteLine(puzzle.GetPermutation());
            }
            writer.Close();
            //puzzle.Save("result.png");
            //Console.WriteLine(puzzle.GetString());
            //System.Threading.Thread.Sleep(10000);
        }
    }
}
