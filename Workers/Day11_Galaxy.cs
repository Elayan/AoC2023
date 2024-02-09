using System;
using System.Linq;
using AoC2023.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day11Galaxy : WorkerBase
    {
        private SpaceMap _originalSpace;
        public override object Data => _originalSpace;

        protected override void ProcessDataLines()
        {
            _originalSpace = new SpaceMap(DataLines.Select(l => l.Select(c => c).ToArray()).ToArray());
        }

        public long ExpansionRate { get; set; } = 1;

        protected override long WorkOneStar_Implementation()
        {
            return SoarThroughTheGalaxies();
        }

        protected override long WorkTwoStars_Implementation()
        {
            return SoarThroughTheGalaxies();
        }

        private long SoarThroughTheGalaxies()
        {
            var expandedSpace = GenerateExpandedSpace();
            if (ExpansionRate < 1000)
                Logger.Log(expandedSpace.ToString());

            var galaxies = expandedSpace.Galaxies.ToArray();

            long distanceSum = 0;
            for (var i = 0; i < galaxies.Length; i++)
            {
                var g1 = galaxies[i];
                for (var j = i + 1; j < galaxies.Length; j++)
                {
                    var g2 = galaxies[j];
                    distanceSum += Math.Abs(g1.Coordinates.Row - g2.Coordinates.Row) +
                                  Math.Abs(g1.Coordinates.Col - g2.Coordinates.Col);
                }
            }

            Logger.Log($"All distances after expansion have been summed = {distanceSum}", SeverityLevel.Always);
            return distanceSum;
        }

        private SpaceMap GenerateExpandedSpace()
        {
            var expandedGalaxies = _originalSpace.Galaxies.ToArray();
            long expandedRowCount = _originalSpace.RowCount;
            long expandedColCount = _originalSpace.ColCount;

            for (long row = 0; row < expandedRowCount; row++)
            {
                if (expandedGalaxies.Any(d => d.Coordinates.Row == row))
                {
                    Logger.Log($"Presence of galaxies in row {row}.");
                    continue; // there's a galaxy on this row, we won't expand it
                }

                Logger.Log($"Expanding row {row} to {row + ExpansionRate}");
                var fartherGalaxies = expandedGalaxies.Where(g => g.Coordinates.Row > row).ToArray();
                foreach (var fartherGalaxy in fartherGalaxies)
                    fartherGalaxy.Coordinates.Row += ExpansionRate;

                row += ExpansionRate;
                expandedRowCount += ExpansionRate;
            }

            for (long col = 0; col < expandedColCount; col++)
            {
                if (expandedGalaxies.Any(d => d.Coordinates.Col == col))
                {
                    Logger.Log($"Presence of galaxies in col {col}.");
                    continue; // there's a galaxy on this col, we won't expand it
                }

                Logger.Log($"Expanding col {col} to {col + ExpansionRate}");
                var fartherGalaxies = expandedGalaxies.Where(g => g.Coordinates.Col > col).ToArray();
                foreach (var fartherGalaxy in fartherGalaxies)
                    fartherGalaxy.Coordinates.Col += ExpansionRate;

                col += ExpansionRate;
                expandedColCount += ExpansionRate;
            }

            return new SpaceMap(expandedGalaxies.ToArray(), expandedRowCount, expandedColCount, true);
        }
    }
}