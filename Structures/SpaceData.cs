using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoCTools.Frame;
using AoCTools.Frame.TwoDimensions;

namespace AoC2023.Structures
{
    public class SpaceMap
    {
        public SpaceMap(char[][] map, bool isExpanded = false)
        {
            var galaxies = new List<SpaceGalaxy>();
            for (var i = 0; i < map.Length; i++)
            {
                for (var j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == '#')
                        galaxies.Add(new SpaceGalaxy(i, j));
                }
            }
            Galaxies = galaxies.ToArray();
            RowCount = map.Length;
            ColCount = map[0].Length;

            IsExpanded = isExpanded;
        }
        public SpaceMap(SpaceGalaxy[] galaxies, long rowCount, long colCount, bool isExpanded = false)
        {
            Galaxies = galaxies;
            RowCount = rowCount;
            ColCount = colCount;

            IsExpanded = isExpanded;
        }

        public SpaceGalaxy[] Galaxies { get; private set; }
        public long RowCount { get; private set; }
        public long ColCount { get; private set; }

        public bool IsExpanded { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"==={(IsExpanded ? " EXPANDED" : string.Empty)} SPACE MAP ===");
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColCount; j++)
                    sb.Append(Galaxies.Any(g => g.Coordinates.Row == i && g.Coordinates.Col == j) ? "#" : ".");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    public class SpaceGalaxy
    {
        public SpaceGalaxy(int row, int col)
        {
            Coordinates = new Coordinates(row, col);
        }

        public SpaceGalaxy(SpaceGalaxy copyFrom)
        {
            Coordinates = new Coordinates(copyFrom.Coordinates.Row, copyFrom.Coordinates.Col);
        }

        public Coordinates Coordinates { get; private set; }
    }
}