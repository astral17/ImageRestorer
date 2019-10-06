using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ImageRestorer
{
    public class SolutionMixer
    {
        private class PuzzleSolution
        {
            public string name;
            public int[] permutation;
            public Int64 score;
        }
        private readonly SortedDictionary<string, PuzzleSolution> solutions = new SortedDictionary<string, PuzzleSolution>();
        public SolutionMixer()
        {

        }
        public SolutionMixer(string imagesPath, int tileSize, string solutionPath)
        {
            AddAllFrom(imagesPath, tileSize, solutionPath);
        }
        public void AddAllFrom(string imagesPath, int tileSize, string solutionPath)
        {
            using (StreamReader reader = new StreamReader(solutionPath))
            {
                while (!reader.EndOfStream)
                {
                    string file = reader.ReadLine();
                    int[] permutation = reader.ReadLine().Trim().Split(' ').Select(x => int.Parse(x)).ToArray();
                    Puzzle puzzle = new Puzzle(Path.Combine(imagesPath, file), tileSize);
                    puzzle.SetPermutation(permutation);
                    Add(puzzle, Path.GetFileName(file));
                }
            }
        }
        public void Add(Puzzle puzzle, string puzzleName)
        {
            PuzzleSolution solution = new PuzzleSolution
            {
                name = puzzleName,
                permutation = puzzle.GetPermutation(),
                score = puzzle.GetTotalScore(),
            };
            if (solutions.ContainsKey(puzzleName))
            {
                if (solution.score < solutions[puzzleName].score)
                    solutions[puzzleName] = solution;
            }
            else
                solutions.Add(puzzleName, solution);
        }
        public void Add(string puzzlePath, int tileSize, int[] permutation)
        {
            Puzzle puzzle = new Puzzle(puzzlePath, tileSize);
            puzzle.SetPermutation(permutation);
            Add(puzzle, Path.GetFileName(puzzlePath));
        }
        public void Add(string puzzlePath, int tileSize, string permutationPath)
        {
            Puzzle puzzle = new Puzzle(puzzlePath, tileSize);
            puzzle.SetPermutation(permutationPath);
            Add(puzzle, Path.GetFileName(puzzlePath));
        }
        public Int64 GetTotalScore()
        {
            Int64 score = 0;
            foreach (KeyValuePair<string, PuzzleSolution> kv in solutions)
            {
                score += kv.Value.score;
            }
            return score;
        }
        public void Save(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (KeyValuePair<string, PuzzleSolution> kv in solutions)
                {
                    writer.WriteLine(kv.Key);
                    writer.WriteLine(String.Join(" ", kv.Value.permutation.Select(x => x.ToString())));
                }
            }
        }
        public void Debug(string imagesPath, int tileSize)
        {
            Directory.CreateDirectory("mix");
            foreach (KeyValuePair<string, PuzzleSolution> kv in solutions)
            {
                Puzzle puzzle = new Puzzle(Path.Combine(imagesPath, kv.Key), tileSize);
                puzzle.SetPermutation(kv.Value.permutation);
                puzzle.Save(String.Format("mix/{0}", kv.Key));
            }

        }
    }
}
