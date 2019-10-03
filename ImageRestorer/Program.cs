using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

using System.Threading;

namespace ImageRestorer
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            DateTime stime = DateTime.Now;
            Puzzle puzzl2e = new Puzzle("0000.png", 16);
            //puzzl2e.Swap(3, 4, 0, 0);
            //Console.WriteLine(puzzl2e.tiles[0, 0].index);
            //Solver.NaiveSolve(puzzl2e);
            //Solver.BFSSolve(puzzl2e);
            Solver.LinesSolve(puzzl2e);
            puzzl2e.Save("result.png");
            Console.WriteLine("Elapsed: {0}", (((TimeSpan)(DateTime.Now - stime)).TotalMilliseconds).ToString());
            System.Threading.Thread.Sleep(int.MaxValue);
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
            //*
            {

                // StreamWriter writer = new StreamWriter("resultC2_lines.txt");
                /*Directory.CreateDirectory("results64NA");
                Directory.CreateDirectory("results64");
                Directory.CreateDirectory("results64t");
                Directory.CreateDirectory("results64VH");*/
                //Directory.CreateDirectory("results64LI");

                Thread[] threads = new Thread[3];

                threads[0] = new Thread(delegate ()
                {
                    StreamWriter writer = new StreamWriter("resultC2_lines_1800.txt");
                    foreach (string file in Directory.GetFiles("D:\\tasks\\shuffled-images-data\\data_test1_blank\\16"))
                    {
                        if (Path.GetFileNameWithoutExtension(file) == "1900")
                            break;
                        Console.WriteLine(file);
                        Puzzle puzzle = new Puzzle(file, 16);
                        Solver.LinesSolve(puzzle);
                        writer.WriteLine(Path.GetFileName(file));
                        writer.WriteLine(puzzle.GetPermutation());
                    }
                    writer.Close();
                });

                threads[1] = new Thread(delegate ()
                {
                    StreamWriter writer = new StreamWriter("resultC2_lines_1900.txt");
                    bool skip = true;
                    foreach (string file in Directory.GetFiles("D:\\tasks\\shuffled-images-data\\data_test1_blank\\16"))
                    {
                        if (skip && Path.GetFileNameWithoutExtension(file) == "1900")
                            skip = false;
                        if (Path.GetFileNameWithoutExtension(file) == "2000")
                            break;
                        if (skip)
                            continue;
                        Console.WriteLine(file);
                        Puzzle puzzle = new Puzzle(file, 16);
                        Solver.LinesSolve(puzzle);
                        writer.WriteLine(Path.GetFileName(file));
                        writer.WriteLine(puzzle.GetPermutation());
                    }
                    writer.Close();
                });

                threads[2] = new Thread(delegate ()
                {
                    StreamWriter writer = new StreamWriter("resultC2_lines_2000.txt");
                    bool skip = true;
                    foreach (string file in Directory.GetFiles("D:\\tasks\\shuffled-images-data\\data_test1_blank\\16"))
                    {
                        if (skip && Path.GetFileNameWithoutExtension(file) == "2000")
                            skip = false;
                        if (Path.GetFileNameWithoutExtension(file) == "2100")
                            break;
                        if (skip)
                            continue;
                        Console.WriteLine(file);
                        Puzzle puzzle = new Puzzle(file, 16);
                        Solver.LinesSolve(puzzle);
                        writer.WriteLine(Path.GetFileName(file));
                        writer.WriteLine(puzzle.GetPermutation());
                    }
                    writer.Close();
                });

                threads[0].Start();
                threads[1].Start();
                threads[2].Start();

                foreach (Thread thread in threads)
                {
                    thread.Join();
                }
                return;

                foreach (string file in Directory.GetFiles("D:\\tasks\\shuffled-images-data\\data_test1_blank\\16"))
                {
                    //string file = "D:\\tasks\\shuffled-images-data\\data_test1_blank\\64\\2699.png";
                    Console.WriteLine(file);
                    // // writer.WriteLine(Path.GetFileName(file));
                    Puzzle puzzle = new Puzzle(file, 16);
                    /*
                    Solver.NaiveSolve(puzzle);
                    puzzle.Save(String.Format("results64NA/{0}", Path.GetFileName(file)));
                    Solver.BFSSolve(puzzle);
                    puzzle.Save(String.Format("results64/{0}", Path.GetFileName(file)));
                    Solver.VHSolve(puzzle);
                    puzzle.Save(String.Format("results64VH/{0}", Path.GetFileName(file)));*/
                    //Solver.BFSSolve(puzzle);
                    Solver.LinesSolve(puzzle);
                    //puzzle.Save(String.Format("results64LI/{0}", Path.GetFileName(file)));
                    // // writer.WriteLine(puzzle.GetPermutation());
                }
                // // writer.Close();
            }
            return;//*/
            {
                StreamWriter writer = new StreamWriter("resultTMP.txt");

                foreach (string file in Directory.GetFiles("D:\\tasks\\shuffled-images-data\\data_test1_blank\\64"))
                {
                    //string file = "D:\\tasks\\shuffled-images-data\\data_test1_blank\\64\\2699.png";
                    Console.WriteLine(file);
                    writer.WriteLine(Path.GetFileName(file));
                    Puzzle puzzle = new Puzzle(file, 64);
                    Solver.NaiveSolve(puzzle);
                    //Solver.BFSSolve(puzzle);
                    writer.WriteLine(puzzle.GetPermutation());
                }
                writer.Close();
            }
            //puzzle.Save("result.png");
            //Console.WriteLine(puzzle.GetString());
            //System.Threading.Thread.Sleep(10000);
        }
    }
}
