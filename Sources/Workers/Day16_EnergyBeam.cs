using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoC2023.Structures;
using AoC2023.Structures.Energy;
using AoCTools.Frame.Map.Extensions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day16EnergyBeam : WorkerBase
    {
        private EnergyMap _map;
        public override object Data => _map;

        protected override void ProcessDataLines()
        {
            _map = new EnergyMap(DataLines.Select(l => l.ToArray()).ToArray());
        }

        protected override long WorkOneStar_Implementation()
        {
            SendBeam(
                _map,
                new BeamStep
                {
                    From = CardinalDirection.West,
                    CellCoord = new Coordinates(0, 0)
                },
                true);
            LogEnergized(_map);

            var energizedCount = _map.EnergizedCellCount;
            Logger.Log($"Energized {energizedCount} cells!", SeverityLevel.Always);
            return energizedCount;
        }

        private static void SendBeam(EnergyMap map, BeamStep firstBeam, bool log)
        {
            var steps = new List<BeamStep> { firstBeam };

            while (steps.Any())
            {
                var step = steps[0];
                steps.Remove(step);

                var cell = map.GetCell(step.CellCoord.Row, step.CellCoord.Col);
                if (cell.EnergizeFrom(step.From))
                {
                    // has already been energized from that direction, stop walking
                    continue;
                }

                ComputeWhereToGo(cell, step, out var to1, out var to2, log);

                var toStep1 = GenerateStep(step, to1, map);
                if (toStep1 != null)
                    steps.Add(toStep1);

                if (to2 == CardinalDirection.None)
                    continue;
                var toStep2 = GenerateStep(step, to2, map);
                if (toStep2 != null)
                    steps.Add(toStep2);
            }
        }

        private static void ComputeWhereToGo(EnergyCell cell, BeamStep step, out CardinalDirection to1, out CardinalDirection to2, bool log)
        {
            to1 = CardinalDirection.None;
            to2 = CardinalDirection.None;

            if (cell.CellType == CellType.Floor
                || cell.CellType == CellType.HorizontalSplitter && step.From.IsHorizontal()
                || cell.CellType == CellType.VerticalSplitter && step.From.IsVertical())
            {
                to1 = step.From.GetOpposite();
            }
            else if (cell.CellType == CellType.HorizontalSplitter)
            {
                to1 = CardinalDirection.West;
                to2 = CardinalDirection.East;
            }
            else if (cell.CellType == CellType.VerticalSplitter)
            {
                to1 = CardinalDirection.North;
                to2 = CardinalDirection.South;
            }
            else if (cell.CellType == CellType.LeftMirror)
            {
                switch (step.From)
                {
                    case CardinalDirection.North:
                        to1 = CardinalDirection.East;
                        break;
                    case CardinalDirection.South:
                        to1 = CardinalDirection.West;
                        break;
                    case CardinalDirection.East:
                        to1 = CardinalDirection.North;
                        break;
                    case CardinalDirection.West:
                        to1 = CardinalDirection.South;
                        break;
                    case CardinalDirection.None:
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            else if (cell.CellType == CellType.RightMirror)
            {
                switch (step.From)
                {
                    case CardinalDirection.North:
                        to1 = CardinalDirection.West;
                        break;
                    case CardinalDirection.South:
                        to1 = CardinalDirection.East;
                        break;
                    case CardinalDirection.East:
                        to1 = CardinalDirection.South;
                        break;
                    case CardinalDirection.West:
                        to1 = CardinalDirection.North;
                        break;
                    case CardinalDirection.None:
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            if (log)
                Logger.Log($"Coming from {step.From} in {step.CellCoord}, being {cell.CellType}, leaving {to1} and {to2}");
        }

        private static BeamStep GenerateStep(BeamStep previousStep, CardinalDirection goingTo, EnergyMap map)
        {
            var toCoord = new Coordinates(previousStep.CellCoord);
            switch (goingTo)
            {
                case CardinalDirection.North:
                    if (toCoord.Row <= 0)
                        return null;
                    toCoord.Row--;
                    break;
                case CardinalDirection.South:
                    if (toCoord.Row >= map.RowCount - 1)
                        return null;
                    toCoord.Row++;
                    break;
                case CardinalDirection.East:
                    if (toCoord.Col >= map.ColCount - 1)
                        return null;
                    toCoord.Col++;
                    break;
                case CardinalDirection.West:
                    if (toCoord.Col <= 0)
                        return null;
                    toCoord.Col--;
                    break;
            }

            return new BeamStep
            {
                From = goingTo.GetOpposite(),
                CellCoord = toCoord
            };
        }

        private static void LogEnergized(EnergyMap map)
        {
            if (Logger.ShowAboveSeverity != SeverityLevel.Never)
                return;

            var sb = new StringBuilder();
            sb.AppendLine("=== ENERGIZED MAP ===");
            foreach (var line in map.MapCells)
            {
                foreach (var cell in line)
                    sb.Append(cell.Energized ? '#' : '.');
                sb.AppendLine();
            }
            Logger.Log(sb.ToString());
        }

        protected override long WorkTwoStars_Implementation()
        {
            var bestEnergizing = -1;
            for (var i = 0; i < _map.RowCount; i++)
            {
                var beam = new BeamStep
                {
                    From = CardinalDirection.West,
                    CellCoord = new Coordinates(i, 0)
                };
                SendBeam(_map, beam, false);
                var energy = _map.EnergizedCellCount;
                if (bestEnergizing == -1 || energy > bestEnergizing)
                    bestEnergizing = energy;
                Logger.Log($"Beam from {beam.From} at {beam.CellCoord} energized {energy} (best = {bestEnergizing})");
                _map.UnEnergize();

                beam = new BeamStep
                {
                    From = CardinalDirection.East,
                    CellCoord = new Coordinates(i, _map.ColCount - 1)
                };
                SendBeam(_map, beam, false);
                energy = _map.EnergizedCellCount;
                if (energy > bestEnergizing)
                    bestEnergizing = energy;
                Logger.Log($"Beam from {beam.From} at {beam.CellCoord} energized {energy} (best = {bestEnergizing})");
                _map.UnEnergize();
            }

            for (var i = 0; i < _map.ColCount; i++)
            {
                var beam = new BeamStep
                {
                    From = CardinalDirection.North,
                    CellCoord = new Coordinates(0, i)
                };
                SendBeam(_map, beam, false);
                var energy = _map.EnergizedCellCount;
                if (bestEnergizing == -1 || energy > bestEnergizing)
                    bestEnergizing = energy;
                Logger.Log($"Beam from {beam.From} at {beam.CellCoord} energized {energy} (best = {bestEnergizing})");
                _map.UnEnergize();

                beam = new BeamStep
                {
                    From = CardinalDirection.South,
                    CellCoord = new Coordinates(_map.RowCount - 1, i)
                };
                SendBeam(_map, beam, false);
                energy = _map.EnergizedCellCount;
                if (energy > bestEnergizing)
                    bestEnergizing = energy;
                Logger.Log($"Beam from {beam.From} at {beam.CellCoord} energized {energy} (best = {bestEnergizing})");
                _map.UnEnergize();
            }

            Logger.Log($"Best beam energize {bestEnergizing} cells!", SeverityLevel.Always);
            return bestEnergizing;
        }
    }
}