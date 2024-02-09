using System.Linq;
using AoCTools.Frame.Map;
using AoCTools.Frame.TwoDimensions.Map;

namespace AoC2023.Structures
{
    public class RockyPlate : CrossLineMap
    {
        public RockyPlate(string[] lines)
            : base(lines)
        {
            RocksOnLines = Lines
                .Select(l => Enumerable.Range(0, l.Length)
                    .Where(r => l[r] == '#')
                    .Select(r => r + 1) // select first ball index
                    .Append(0) // add the fact ball can start at 0
                    .Append(LinesSize + 1) // add the end of line
                    .OrderBy(i => i)
                    .ToArray())
                .ToArray();
            RocksOnColumns = Columns
                .Select(c => Enumerable.Range(0, c.Length)
                    .Where(r => c[r] == '#')
                    .Select(r => r + 1) // select first ball index
                    .Append(0) // add the fact ball can start at 0
                    .Append(ColumnsSize + 1) // add the end of line
                    .OrderBy(i => i)
                    .ToArray())
                .ToArray();
        }

        public int[][] RocksOnLines { get; private set; } // [ line index ] [ rock pos ]
        public int[][] RocksOnColumns { get; private set; } // [ line index ] [ rock pos ]
    }
}