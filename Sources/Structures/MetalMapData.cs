using System;
using System.Collections.Generic;
using System.Linq;
using AoCTools.Frame;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;
using AoCTools.Loggers;

namespace AoC2023.Structures
{
    public enum MetalGroundType
    {
        NorthSouthPipe, // |
        EastWestPipe,   // -
        NorthEastPipe,  // L
        NorthWestPipe,  // J
        SouthWestPipe,  // 7
        SouthEastPipe,  // F
        FlatGround,     // .
        Unknown,
    }

    public class MetalMap : Map<MetalCell>
    {
        public MetalMap(char[][] mapChars) : base(mapChars)
        {
            InitializeNeighbors();
            InitializeStartingPoint();
        }

        public MetalCell StartingPoint { get; private set; }

        public long GetGreatestDistance()
        {
            return AllCells.Max(c => c.DistanceFromStart);
        }

        private void InitializeNeighbors()
        {
            for (var i = 0; i < MapCells.Length; i++)
            {
                for (var j = 0; j < MapCells[i].Length; j++)
                {
                    if (i > 0 && MapCells[i][j].IsLinkedToDirection(CardinalDirection.North))
                    {
                        MapCells[i][j].Neighbors.Add(MapCells[i-1][j]);
                    }

                    if (i < MapCells.Length - 1 && MapCells[i][j].IsLinkedToDirection(CardinalDirection.South))
                    {
                        MapCells[i][j].Neighbors.Add(MapCells[i+1][j]);
                    }

                    if (j > 0 && MapCells[i][j].IsLinkedToDirection(CardinalDirection.West))
                    {
                        MapCells[i][j].Neighbors.Add(MapCells[i][j-1]);
                    }

                    if (j < MapCells[i].Length - 1 && MapCells[i][j].IsLinkedToDirection(CardinalDirection.East))
                    {
                        MapCells[i][j].Neighbors.Add(MapCells[i][j+1]);
                    }
                }
            }
        }

        private void InitializeStartingPoint()
        {
            StartingPoint = AllCells.First(c => c.GroundType == MetalGroundType.Unknown);
            Logger.Log($"Starting point at {StartingPoint.Coordinates}");

            var neighbors = AllCells.Where(c => c.Neighbors.Contains(StartingPoint)).ToList();
            StartingPoint.Neighbors.AddRange(neighbors);
            Logger.Log($"Starting point has {neighbors.Count} neighbors");

            StartingPoint.IsPipe = true;
            StartingPoint.UpdateTypeFromNeighbors();
        }
    }

    public class MetalCell : MapCell<MetalGroundType>
    {
        public MetalCell(char c, int x, int y) : base (CharToType(c), x, y)
        {
        }

        public MetalGroundType GroundType => Content;
        public List<MetalCell> Neighbors { get; } = new List<MetalCell>();

        public long DistanceFromStart { get; set; }

        public bool IsPipe { get; set; }
        public bool IsNest { get; set; }

        public bool IsEastPipe => IsPipe
                                  && (GroundType == MetalGroundType.NorthEastPipe
                                      || GroundType == MetalGroundType.SouthEastPipe
                                      || GroundType == MetalGroundType.EastWestPipe);

        public bool IsLinkedToDirection(CardinalDirection direction)
        {
            return
                direction == CardinalDirection.North
                    && (GroundType == MetalGroundType.NorthEastPipe
                        || GroundType == MetalGroundType.NorthSouthPipe
                        || GroundType == MetalGroundType.NorthWestPipe)
                || direction == CardinalDirection.South
                    && (GroundType == MetalGroundType.NorthSouthPipe
                        || GroundType == MetalGroundType.SouthEastPipe
                        || GroundType == MetalGroundType.SouthWestPipe)
                || direction == CardinalDirection.East
                    && (GroundType == MetalGroundType.NorthEastPipe
                        || GroundType == MetalGroundType.SouthEastPipe
                        || GroundType == MetalGroundType.EastWestPipe)
                || direction == CardinalDirection.West
                    && (GroundType == MetalGroundType.NorthWestPipe
                        || GroundType == MetalGroundType.SouthWestPipe
                        || GroundType == MetalGroundType.EastWestPipe);
        }

        private static MetalGroundType CharToType(char c)
        {
            switch (c)
            {
                case '|': return MetalGroundType.NorthSouthPipe;
                case '-': return MetalGroundType.EastWestPipe;
                case 'L': return MetalGroundType.NorthEastPipe;
                case 'J': return MetalGroundType.NorthWestPipe;
                case '7': return MetalGroundType.SouthWestPipe;
                case 'F': return MetalGroundType.SouthEastPipe;
                case '.': return MetalGroundType.FlatGround;
                case 'S': return MetalGroundType.Unknown;
            }
            throw new Exception("Read unexpected character.");
        }

        private string TypeToChar(MetalGroundType type)
        {
            switch (type)
            {
                case MetalGroundType.NorthSouthPipe: return "|";
                case MetalGroundType.EastWestPipe:   return "-";
                case MetalGroundType.NorthEastPipe:  return "L";
                case MetalGroundType.NorthWestPipe:  return "J";
                case MetalGroundType.SouthWestPipe:  return "7";
                case MetalGroundType.SouthEastPipe:  return "F";
                case MetalGroundType.FlatGround:     return ".";
                default: return "#";
            }
        }

        public override string ToString()
        {
            return IsNest ? "0" : TypeToChar(GroundType);
        }

        public void UpdateTypeFromNeighbors()
        {
            var isStartConnectingNorth = Neighbors.Any(n => n.Coordinates.Row == Coordinates.Row - 1);
            var isStartConnectingSouth = Neighbors.Any(n => n.Coordinates.Row == Coordinates.Row + 1);
            var isStartConnectingWest = Neighbors.Any(n => n.Coordinates.Col == Coordinates.Col - 1);
            var isStartConnectingEast = Neighbors.Any(n => n.Coordinates.Col == Coordinates.Col + 1);
            if (isStartConnectingNorth)
            {
                if (isStartConnectingSouth)
                    Content = MetalGroundType.NorthSouthPipe;
                else if (isStartConnectingEast)
                    Content = MetalGroundType.NorthEastPipe;
                else if (isStartConnectingWest)
                    Content = MetalGroundType.NorthWestPipe;
            }
            else if (isStartConnectingSouth)
            {
                if (isStartConnectingEast)
                    Content = MetalGroundType.SouthEastPipe;
                else if (isStartConnectingWest)
                    Content = MetalGroundType.SouthWestPipe;
            }
            else Content = MetalGroundType.EastWestPipe;
            Logger.Log($"Cell at {Coordinates} updated Type to {GroundType}");
        }
    }
}