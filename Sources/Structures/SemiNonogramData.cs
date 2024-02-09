using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC2023.Structures
{
    public class NonogramLine
    {
        public NonogramLine(string line, int[] serie)
        {
            Size = line.Length;
            Serie = serie;
            FilledIndexes = Enumerable.Range(0, line.Length)
                .Where(r => line[r] == '#').ToArray();
            EmptyIndexes = Enumerable.Range(0, line.Length)
                .Where(r => line[r] == '.').ToArray();

            ComputeFillableIndexes();
        }

        public int[] Serie { get; private set; }
        public int Size { get; private set; }
        public int[] FilledIndexes { get; private set; }
        public int[] EmptyIndexes { get; private set; }
        public Dictionary<int, int> FillableIndexes { get; private set; } = new Dictionary<int, int>();

        private Regex _fillableRegex = new Regex(@"(#|\?)+", RegexOptions.Compiled);

        private void ComputeFillableIndexes()
        {
            FillableIndexes.Clear();
            var s = BuildRawString();
            var matches = _fillableRegex.Matches(s);
            foreach (Match match in matches)
            {
                for (var i = match.Index; i < match.Index + match.Length; i++)
                {
                    FillableIndexes.Add(i, match.Index + match.Length - i);
                }
            }
        }

        public override string ToString()
        {
            var line = Size > 50 ? "<long line>" : BuildRawString();
            var serie = Serie.Length > 20 ? "<long serie>" : string.Join(", ", Serie);
            return $"{line} [{serie}] ({Size})";
        }

        private string BuildRawString()
        {
            var sb = Enumerable.Repeat('?', Size).ToArray();
            foreach (var filledIndex in FilledIndexes)
                sb[filledIndex] = '#';
            foreach (var emptyIndex in EmptyIndexes)
                sb[emptyIndex] = '.';
            return string.Join("", sb);
        }

        public void Unfold()
        {
            var newSerie = new List<int>(Serie);
            newSerie.AddRange(Serie);
            newSerie.AddRange(Serie);
            newSerie.AddRange(Serie);
            newSerie.AddRange(Serie);
            Serie = newSerie.ToArray();

            var newFilledIndexes = new List<int>(FilledIndexes);
            newFilledIndexes.AddRange(FilledIndexes.Select(i => i + Size + 1));
            newFilledIndexes.AddRange(FilledIndexes.Select(i => i + 2 * Size + 2));
            newFilledIndexes.AddRange(FilledIndexes.Select(i => i + 3 * Size + 3));
            newFilledIndexes.AddRange(FilledIndexes.Select(i => i + 4 * Size + 4));
            FilledIndexes = newFilledIndexes.ToArray();

            var newEmptyIndexes = new List<int>(EmptyIndexes);
            newEmptyIndexes.AddRange(EmptyIndexes.Select(i => i + Size + 1));
            newEmptyIndexes.AddRange(EmptyIndexes.Select(i => i + 2 * Size + 2));
            newEmptyIndexes.AddRange(EmptyIndexes.Select(i => i + 3 * Size + 3));
            newEmptyIndexes.AddRange(EmptyIndexes.Select(i => i + 4 * Size + 4));
            EmptyIndexes = newEmptyIndexes.ToArray();

            Size = Size * 5 + 4;
            ComputeFillableIndexes();
        }

        public bool Validate(Dictionary<int, int> blocks)
        {
            foreach (var filledIndex in FilledIndexes)
            {
                if (blocks.Any(b => b.Key <= filledIndex && filledIndex < b.Key + b.Value))
                    continue;
                return false;
            }

            foreach (var emptyIndex in EmptyIndexes)
            {
                if (blocks.Any(b => b.Key <= emptyIndex && emptyIndex < b.Key + b.Value))
                    return false;
            }

            return true;
        }
    }

    public class SemiNonogram
    {
        public List<NonogramLine> Lines { get; } = new List<NonogramLine>();

        public void Unfold()
        {
            foreach (var line in Lines)
                line.Unfold();
        }

        public override string ToString()
        {
            if (Lines.Count > 100)
                return "Too many lines to display!";

            var sb = new StringBuilder();
            sb.AppendLine("=== SEMI NONOGRAM ===");
            foreach (var line in Lines)
                sb.AppendLine(line.ToString());
            return sb.ToString();
        }
    }
}