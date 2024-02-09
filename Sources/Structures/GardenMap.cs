using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoCTools.Frame;
using AoCTools.Frame.Map;
using AoCTools.Frame.Map.Extensions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;

namespace AoC2023.Structures
{
    public enum GardenCellType
    {
        Plot,
        Rock
    }

    public class GardenMap : Map<GardenCell>
    {
        public GardenMap(char[][] mapChars) : base(ConvertCharsToCells(mapChars))
        {
            StartingPoint = AllCells.First(c => c.IsStartingPlot);
        }

        private static GardenCell[][] ConvertCharsToCells(char[][] mapChars)
        {
            var lines = new List<GardenCell[]>();
            for (var row = 0; row < mapChars.Length; row++)
            {
                var line = mapChars[row];
                var cells = new List<GardenCell>();
                for (var col = 0; col < line.Length; col++)
                {
                    var c = line[col];
                    var cell = new GardenCell(c, row, col);
                    if (c == 'S')
                        cell.IsStartingPlot = true;
                    cells.Add(cell);
                }
                lines.Add(cells.ToArray());
            }
            return lines.ToArray();
        }

        protected override string LogTitle => "=== GARDEN ===";

        public GardenCell StartingPoint { get; }

        public GardenCell[] AllPlots =>
            AllCells.Where(c => c.CellType == GardenCellType.Plot).ToArray();

        public string ToDistanceString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== WALKED GARDEN ===");
            foreach (var line in MapCells)
            {
                var topLane = new List<string>();
                var midLane = new List<string>();
                var botLane = new List<string>();
                foreach (var cell in line.Cast<GardenCell>())
                {
                    var upDistance = cell.GetDistanceForExtension(new GardenExtension(CardinalDirection.North, 1, CardinalDirection.None, 0));
                    var up = upDistance != -1 ? $"{upDistance:00}" : $"{cell.RawContent}{cell.RawContent}";
                    topLane.Add($"   {up}   ");

                    var leftDistance = cell.GetDistanceForExtension(new GardenExtension(CardinalDirection.West, 1, CardinalDirection.None, 0));
                    var left = leftDistance != -1 ? $"{leftDistance:00}" : $"{cell.RawContent}{cell.RawContent}";
                    var centralDistance = cell.GetDistanceForExtension(GardenExtension.Central);
                    var central = centralDistance != -1 ? $"{centralDistance:00}" : $"{cell.RawContent}{cell.RawContent}";
                    var rightDistance = cell.GetDistanceForExtension(new GardenExtension(CardinalDirection.East, 1, CardinalDirection.None, 0));
                    var right = rightDistance != -1 ? $"{rightDistance:00}" : $"{cell.RawContent}{cell.RawContent}";
                    midLane.Add($"{left}-{central}-{right}");

                    var downDistance = cell.GetDistanceForExtension(new GardenExtension(CardinalDirection.South, 1, CardinalDirection.None, 0));
                    var down = downDistance != -1 ? $"{downDistance:00}" : $"{cell.RawContent}{cell.RawContent}";
                    botLane.Add($"   {down}   ");
                }
                sb.Append(string.Join(" ", topLane));
                sb.AppendLine();
                sb.Append(string.Join(" ", midLane));
                sb.AppendLine();
                sb.Append(string.Join(" ", botLane));
                sb.AppendLine();

                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    public class GardenCell : MapCell<GardenCellType>
    {
        public GardenCell(char c, int row, int y) : base(CharToType(c), row, y)
        {
        }

        public bool IsStartingPlot { get; set; }
        public GardenCellType CellType => Content;
        public List<GardenDistance> Distances { get; } = new List<GardenDistance>();

        public long GetDistanceForExtension(GardenExtension extension)
        {
            var matching = Distances.FirstOrDefault(d => d.Extension.Equals(extension));
            if (matching == null)
                return -1;

            return matching.Distance;
        }

        public void SetDistanceForExtension(GardenExtension extension, int value)
        {
            var matching = Distances.FirstOrDefault(d => d.Extension.Equals(extension));
            if (matching == null)
            {
                Distances.Add(new GardenDistance
                {
                    Extension = extension.GetCopy(),
                    Distance = value
                });
            }
            else
            {
                matching.Distance = value;
            }
        }

        private static GardenCellType CharToType(char c)
        {
            switch (c)
            {
                case 'S':
                case '.':
                    return GardenCellType.Plot;
                case '#':
                    return GardenCellType.Rock;
                default:
                    throw new Exception($"Unknown garden cell type '{c}'");
            }
        }
    }

    public class GardenStep
    {
        public GardenCell Cell { get; set; }
        public int Steps { get; set; } = -1;
        public GardenExtension Extension { get; set; }
    }

    public class GardenExtension : IEquatable<GardenExtension>
    {
        public static readonly GardenExtension Central = new GardenExtension(CardinalDirection.None, 0, CardinalDirection.None, 0);

        private CardinalDirection _direction1;
        private int _power1;

        private CardinalDirection _direction2;
        private int _power2;

        public int GetComputingPower()
        {
            if (_direction1 == CardinalDirection.None || _direction2 == CardinalDirection.None)
                return 1;
            return _power1 + _power2 - 1;
        }

        public GardenExtension(CardinalDirection dir1, int pow1, CardinalDirection dir2, int pow2)
        {
            if (dir1 < dir2)
            {
                _direction1 = dir1;
                _power1 = pow1;
                _direction2 = dir2;
                _power2 = pow2;
            }
            else
            {
                _direction1 = dir2;
                _power1 = pow2;
                _direction2 = dir1;
                _power2 = pow1;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GardenExtension)obj);
        }

        public bool Equals(GardenExtension other)
        {
            //return other != null
            //    && _direction1 == other._direction1
            //    && _direction2 == other._direction2
            //    && _power1 == other._power1
            //    && _power2 == other._power2;
            return other != null
                   && _direction1 == other._direction1
                   && _direction2 == other._direction2
                   && _power1 + _power2 == other._power1 + other._power2;
        }

        public override int GetHashCode()
        {
            return (int)_direction1 * _power1 * (int)_direction2 * _power2;
        }

        public GardenExtension GetCopy()
        {
            return new GardenExtension(_direction1, _power1, _direction2, _power2);
        }

        public bool CanExtend(CardinalDirection direction)
        {
            return _direction1 == CardinalDirection.None
                   || _direction1 == direction
                   || _direction2 == direction
                   || _direction2 == CardinalDirection.None && direction != _direction1.GetOpposite();
        }

        public void Extend(CardinalDirection direction)
        {
            if (_direction1 == CardinalDirection.None || _direction1 == direction)
            {
                _direction1 = direction;
                _power1++;
            }
            else
            {
                if (_direction1 < direction)
                {
                    _direction2 = direction;
                    _power2++;
                }
                else
                {
                    _direction2 = _direction1;
                    _power2 = _power1;
                    _direction1 = direction;
                    _power1 = 1;
                }
            }
        }

        public override string ToString()
        {
            return $"{_direction1.ToShortName()}{_power1}-{_direction2.ToShortName()}{_power2}";
        }
    }

    public class GardenDistance
    {
        public GardenExtension Extension { get; set; }
        public long Distance { get; set; }

        public override string ToString()
        {
            return $"[{Extension}]={Distance}";
        }
    }
}