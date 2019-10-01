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
            Puzzle puzzl2e = new Puzzle("0000.png", 8);
            Solver.NaiveSolve(puzzl2e);
            puzzl2e.Save("result.png");
            //StreamWriter sw = new StreamWriter("output.txt");
            //sw.WriteLine(puzzl2e.GetPermutation());
            //sw.Close();
            return;//*/
            /*
            Puzzle puzzl2e = new Puzzle("1200.png", 64);
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
                puzzl2e.Swap(index % 8, index / 8, pos % 8, pos / 8);
                puzzl2e.Save(String.Format("results2/{0}.png", index));
                index++;
            }
            puzzl2e.Save("result.png");
            StreamWriter sw = new StreamWriter("output2.txt");
            sw.WriteLine(puzzl2e.GetPermutation());
            sw.Close();
            return;//*/

            StreamWriter writer = new StreamWriter("result4.txt");

            foreach (string file in Directory.GetFiles("D:\\tasks\\shuffled-images-data\\data_test1_blank\\64"))
            {
                //string file = "D:\\tasks\\shuffled-images-data\\data_test1_blank\\64\\2699.png";
                Console.WriteLine(file);
                writer.WriteLine(Path.GetFileName(file));
                Puzzle puzzle = new Puzzle(file, 64);
                Solver.NaiveSolve(puzzle);
                writer.WriteLine(puzzle.GetPermutation());
            }
            writer.Close();
            //puzzle.Save("result.png");
            //Console.WriteLine(puzzle.GetString());
            //System.Threading.Thread.Sleep(10000);
        }
    }
}
