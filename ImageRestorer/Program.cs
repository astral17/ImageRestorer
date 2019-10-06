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
        private class SolveFromToData
        {
            public int tileSize;
            public string from, to;

        }
        static void SolveFromTo(object obj)
        {
            int tileSize = (obj as SolveFromToData).tileSize;
            string from = (obj as SolveFromToData).from, to = (obj as SolveFromToData).to;
            bool skip = true;

            StreamWriter writer = new StreamWriter(String.Format("result_thread_{0}.txt", from));
            foreach (string file in Directory.GetFiles(String.Format("D:\\tasks\\shuffled-images-data\\data_test1_blank\\{0}", tileSize.ToString())))
            {
                if (skip && Path.GetFileNameWithoutExtension(file) == from)
                    skip = false;
                if (Path.GetFileNameWithoutExtension(file) == to)
                    break;
                if (skip)
                    continue;
                Console.WriteLine(file);
                Puzzle puzzle = new Puzzle(file, tileSize);
                //Solver.LinesSolve(puzzle);
                Solver.BFSSolve(puzzle);
                writer.WriteLine(Path.GetFileName(file));
                writer.WriteLine(puzzle.GetPermutationString());
            }
            writer.Close();
        }
        static void Main(string[] args)
        {
            /*
            DateTime stime = DateTime.Now;
            Puzzle puzzl2e = new Puzzle("0600.png", 32);
            //puzzl2e.Swap(3, 4, 0, 0);
            //Console.WriteLine(puzzl2e.tiles[0, 0].index);
            /*
            Directory.CreateDirectory("tmp");
            Solver.NaiveSolve(puzzl2e);
            Console.WriteLine("NAI: {0}", puzzl2e.GetTotalScore());
            puzzl2e.Save("tmp/resultNA.png");
            Solver.BFSSolve(puzzl2e);
            Console.WriteLine("BFS: {0}", puzzl2e.GetTotalScore());
            puzzl2e.Save("tmp/resultBF.png");
            Solver.VHSolve(puzzl2e);
            Console.WriteLine("VH : {0}", puzzl2e.GetTotalScore());
            puzzl2e.Save("tmp/resultVH.png");
            Solver.LinesSolve(puzzl2e);
            Console.WriteLine("LIN: {0}", puzzl2e.GetTotalScore());
            puzzl2e.Save("tmp/resultLI.png");*/
            //puzzl2e.Save("result.png");
            /*
            Solver.BFSSolve(puzzl2e);
            puzzl2e.Save("result.png");
            Console.WriteLine("BFS: {0}", puzzl2e.GetTotalScore());
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

            if (true)
            {
                //string[] files = { "resultA2.txt", "resultA2_gray.txt", "resultA2_lines.txt", "resultTMP.txt", "resultA2_good.txt" };
                //string[] files = { "resultA2_mix_2.txt", "resultA2_mix_n.txt" };
                //string[] files = { "resultC2.txt", "resultC2_lines2.txt", "resultC2_tmp.txt", "resultC2_bfs.txt" };
                //string[] files = { "resultC2_bfs.txt", "resultC2.txt", "resultC2_lines2.txt", "resultC2_tmp.txt", "resultC2_bfs_2.txt" };
                //string[] files = { "resultC2_mix_1e.txt", "resultC2_mix_se.txt" };
                //string[] files = { "resultC2_mix_se.txt", "resultC2_mix_1e.txt" };
                string[] files = { "resultB2.txt", "resultB2_1.txt" };
                SolutionMixer mixer = new SolutionMixer();
                foreach (string file in files)
                {
                    mixer.AddAllFrom("D:\\tasks\\shuffled-images-data\\data_test1_blank\\32", 32, file);
                    Console.WriteLine(mixer.GetTotalScore());
                }
                mixer.Save("resultB2_mix.txt");
                //mixer.Save("resultC2_mix.txt");
                //mixer.Debug("D:\\tasks\\shuffled-images-data\\data_test1_blank\\64", 16);
                Console.WriteLine("Finish");
                Thread.Sleep(int.MaxValue);
                return;
            }
            if (false)
            {
                StreamWriter writer = new StreamWriter("resultA2_good.txt");
                Directory.CreateDirectory("good64");
                foreach (string file in Directory.GetFiles("D:\\tasks\\shuffled-images-data\\data_test1_blank\\64"))
                {
                    Console.WriteLine(file);
                    Puzzle puzzle = new Puzzle(file, 64);
                    Solver.BFSSolve(puzzle);
                    writer.WriteLine(Path.GetFileName(file));
                    writer.WriteLine(puzzle.GetPermutationString());
                    puzzle.Save(String.Format("good64/{0}", Path.GetFileName(file)));
                }
                writer.Close();
                return;
            }

            /*
            {
                Puzzle puzzl2e = new Puzzle("0600.png", 32);
                puzzl2e.SetPermutation("input.txt");
                puzzl2e.Save("result.png");
            }
            return;

            /*int index = 0;
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
            }*/
            /*
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

                Thread[] threads =
                {
                    new Thread(SolveFromTo),
                    new Thread(SolveFromTo),
                    new Thread(SolveFromTo),
                };

                threads[0].Start(new SolveFromToData
                {
                    tileSize = 32,
                    from = "2100",
                    to = "2200",
                });
                threads[1].Start(new SolveFromToData
                {
                    tileSize = 32,
                    from = "2200",
                    to = "2300",
                });
                threads[2].Start(new SolveFromToData
                {
                    tileSize = 32,
                    from = "2300",
                    to = "2400",
                });

                /*
                threads[0].Start(new SolveFromToData
                {
                    tileSize = 16,
                    from = "1800",
                    to = "1900",
                });
                threads[1].Start(new SolveFromToData
                {
                    tileSize = 16,
                    from = "1900",
                    to = "2000",
                });
                threads[2].Start(new SolveFromToData
                {
                    tileSize = 16,
                    from = "2000",
                    to = "2100",
                });//*/

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
                    writer.WriteLine(puzzle.GetPermutationString());
                }
                writer.Close();
            }
            //puzzle.Save("result.png");
            //Console.WriteLine(puzzle.GetString());
            //System.Threading.Thread.Sleep(10000);
        }
    }
}
