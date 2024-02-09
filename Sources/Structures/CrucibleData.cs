using System;
using System.Collections.Generic;
using System.Linq;
using AoCTools.Frame;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map.Abstracts;

namespace AoC2023.Structures
{
    public class HeatCharMap : Map<HeatCell>
    {
        public HeatCharMap(char[][] mapChars) : base(mapChars)
        { }

        protected override string LogTitle => "=== HEAT MAP ===";

        public void ResetVisits()
        {
            foreach (var cell in AllCells)
            {
                cell.ResetVisits();
            }
        }
    }

    public class HeatCell : MapCell<long>
    {
        public HeatCell(char c, int x, int y) : base(long.Parse($"{c}"), x, y)
        { }

        private readonly List<Tuple<CardinalDirection, int>> _visits = new List<Tuple<CardinalDirection, int>>();

        /// <summary>
        /// Add visit and check if already visited under those conditions.
        /// </summary>
        /// <returns>TRUE if already visited under these conditions</returns>
        public bool Visit(CardinalDirection direction, int directionSteps)
        {
            if (_visits.Any(t => t.Item1 == direction && t.Item2 == directionSteps))
                return true;
            _visits.Add(new Tuple<CardinalDirection, int>(direction, directionSteps));
            return false;
        }

        public void ResetVisits()
        {
            _visits.Clear();
        }
    }

    public class HeatStep : IComparable
    {
        public HeatCell Cell { get; set; }
        public CardinalDirection Direction { get; set; } = CardinalDirection.None;
        public HeatStep PreviousStep { get; set; } = null;
        public long PreviousHeatLoss { get; set; } = 0;
        public long TotalHeatLoss => PreviousHeatLoss + Cell.Content;
        public int StepsThatDirection { get; set; } = 1;

        public int CompareTo(object obj)
        {
            if (!(obj is HeatStep other))
                throw new ArgumentException();

            return TotalHeatLoss.CompareTo(other.TotalHeatLoss);
        }
    }
}