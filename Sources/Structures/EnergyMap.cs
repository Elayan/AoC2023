using System;
using System.Collections.Generic;
using System.Linq;
using AoC2023.Structures.Energy;
using AoCTools.Frame;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;

namespace AoC2023.Structures
{
    namespace Energy
    {
        public enum CellType
        {
            Floor,
            VerticalSplitter,
            HorizontalSplitter,
            LeftMirror,
            RightMirror,
        }
    }

    public class EnergyMap : Map<EnergyCell>
    {
        public EnergyMap(char[][] mapChars) : base(mapChars)
        { }

        protected override string LogTitle => "=== ENERGY MAP ===";

        public int EnergizedCellCount => AllCells.Count(c => c.Energized);

        public void UnEnergize()
        {
            foreach (var cell in AllCells)
            {
                cell.UnEnergize();
            }
        }
    }

    public class EnergyCell : MapCell<CellType>
    {
        public EnergyCell(char c, int x, int col) : base(CharToType(c), x, col)
        {
        }

        public CellType CellType => Content;

        private readonly List<CardinalDirection> _energizedFrom = new List<CardinalDirection>();
        public bool Energized => _energizedFrom.Any();

        /// <summary>
        /// Energize the cell, and returns if it has already been energized from there previously (loop prevention)
        /// </summary>
        /// <returns>Loop prevention: TRUE if has been energized from that direction before.</returns>
        public bool EnergizeFrom(CardinalDirection c)
        {
            if (_energizedFrom.Contains(c))
                return true;
            _energizedFrom.Add(c);
            return false;
        }

        private static CellType CharToType(char c)
        {
            switch (c)
            {
                case '.': return CellType.Floor;
                case '|': return CellType.VerticalSplitter;
                case '-': return CellType.HorizontalSplitter;
                case '/': return CellType.RightMirror;
                case '\\': return CellType.LeftMirror;
            }

            throw new Exception($"Unknown char '{c}'");
        }

        public void UnEnergize()
        {
            _energizedFrom.Clear();
        }
    }

    public class BeamStep
    {
        public CardinalDirection From { get; set; }
        public Coordinates CellCoord { get; set; }
    }
}