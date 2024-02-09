using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoC2023.Structures;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day17Crucible : WorkerBase
    {
        private HeatCharMap _map;
        public override object Data => _map;

        protected override void ProcessDataLines()
        {
            _map = new HeatCharMap(DataLines.Select(l => l.ToArray()).ToArray());
        }

        protected override long WorkOneStar_Implementation()
        {
            return FindTheWay(0, 3);
        }

        protected override long WorkTwoStars_Implementation()
        {
            return FindTheWay(4, 10);
        }

        private long FindTheWay(int minStepsInOneDirection, int maxStepsInOneDirection)
        {
            var lowestHeatLossPath = PushTheCrucible(_map, minStepsInOneDirection, maxStepsInOneDirection);

            if (Logger.ShowAboveSeverity == SeverityLevel.Never)
                LogVisitedPath(_map, lowestHeatLossPath);

            var firstCell = _map.GetCell(0, 0);
            var lowestHeatLoss = lowestHeatLossPath.TotalHeatLoss - firstCell.Content;
            Logger.Log($"Lowest heat loss = {lowestHeatLoss}", SeverityLevel.Always);
            return lowestHeatLoss;
        }

        private static void LogVisitedPath(HeatCharMap charMap, HeatStep lastStep)
        {
            var mapChars = charMap.MapCells.Select(row => row.Select(r => r.RawContent).ToArray()).ToArray();
            var curStep = lastStep;
            while (curStep != null)
            {
                mapChars[curStep.Cell.Coordinates.Row][curStep.Cell.Coordinates.Col] =
                    curStep.Direction == CardinalDirection.North
                        ? '^'
                        : curStep.Direction == CardinalDirection.South
                            ? 'v'
                            : curStep.Direction == CardinalDirection.East
                                ? '>'
                                : curStep.Direction == CardinalDirection.West
                                    ? '<'
                                    : 'o';
                curStep = curStep.PreviousStep;
            }

            var sb = new StringBuilder();
            sb.AppendLine("=== WALKED PATH ===");
            foreach (var row in mapChars)
                sb.AppendLine(string.Join("", row));
            Logger.Log(sb.ToString());
        }

        private static HeatStep PushTheCrucible(HeatCharMap charMap, int minStepsInOneDirection, int maxStepsInOneDirection)
        {
            var startCell = charMap.GetCell(0, 0);
            var start = new HeatStep
            {
                Cell = startCell
            };
            var paths = new List<HeatStep> { start };

            while (true)
            {
                var lowestPath = paths.Min(p => p);
                paths.Remove(lowestPath);

                // actually visit the cell
                if (lowestPath.Cell.Coordinates.Row == charMap.RowCount - 1
                    && lowestPath.Cell.Coordinates.Col == charMap.ColCount - 1)
                {
                    // we found the end
                    return lowestPath;
                }

                if (lowestPath.Cell.Visit(lowestPath.Direction, lowestPath.StepsThatDirection))
                {
                    // we reached a path already visited with less heat loss
                    continue;
                }

                // prepare next steps (condition is a quick prevent from going right back)
                if (lowestPath.Direction != CardinalDirection.North
                    && lowestPath.Cell.Coordinates.Row < charMap.RowCount - 1
                    && !(lowestPath.Direction == CardinalDirection.South && lowestPath.StepsThatDirection >= maxStepsInOneDirection)
                    && (lowestPath.Direction == CardinalDirection.None
                        || !(lowestPath.Direction != CardinalDirection.South && lowestPath.StepsThatDirection < minStepsInOneDirection)))
                {
                    var nextCell = charMap.GetCell(lowestPath.Cell.Coordinates.Row + 1, lowestPath.Cell.Coordinates.Col);
                    paths.Add(new HeatStep
                    {
                        Direction = CardinalDirection.South,
                        Cell = nextCell,
                        PreviousStep = lowestPath,
                        PreviousHeatLoss = lowestPath.TotalHeatLoss,
                        StepsThatDirection = lowestPath.Direction == CardinalDirection.South
                            ? lowestPath.StepsThatDirection + 1 : 1
                    });
                }

                if (lowestPath.Direction != CardinalDirection.South
                    && lowestPath.Cell.Coordinates.Row > 0
                    && !(lowestPath.Direction == CardinalDirection.North && lowestPath.StepsThatDirection >= maxStepsInOneDirection)
                    && (lowestPath.Direction == CardinalDirection.None
                        || !(lowestPath.Direction != CardinalDirection.North && lowestPath.StepsThatDirection < minStepsInOneDirection)))
                {
                    var nextCell = charMap.GetCell(lowestPath.Cell.Coordinates.Row - 1, lowestPath.Cell.Coordinates.Col);
                    paths.Add(new HeatStep
                    {
                        Direction = CardinalDirection.North,
                        Cell = nextCell,
                        PreviousStep = lowestPath,
                        PreviousHeatLoss = lowestPath.TotalHeatLoss,
                        StepsThatDirection = lowestPath.Direction == CardinalDirection.North
                            ? lowestPath.StepsThatDirection + 1 : 1
                    });
                }

                if (lowestPath.Direction != CardinalDirection.East
                    && lowestPath.Cell.Coordinates.Col > 0
                    && !(lowestPath.Direction == CardinalDirection.West && lowestPath.StepsThatDirection >= maxStepsInOneDirection)
                    && (lowestPath.Direction == CardinalDirection.None
                        || !(lowestPath.Direction != CardinalDirection.West && lowestPath.StepsThatDirection < minStepsInOneDirection)))
                {
                    var nextCell = charMap.GetCell(lowestPath.Cell.Coordinates.Row, lowestPath.Cell.Coordinates.Col - 1);
                    paths.Add(new HeatStep
                    {
                        Direction = CardinalDirection.West,
                        Cell = nextCell,
                        PreviousStep = lowestPath,
                        PreviousHeatLoss = lowestPath.TotalHeatLoss,
                        StepsThatDirection = lowestPath.Direction == CardinalDirection.West
                            ? lowestPath.StepsThatDirection + 1 : 1
                    });
                }

                if (lowestPath.Direction != CardinalDirection.West
                    && lowestPath.Cell.Coordinates.Col < charMap.ColCount - 1
                    && !(lowestPath.Direction == CardinalDirection.East && lowestPath.StepsThatDirection >= maxStepsInOneDirection)
                    && (lowestPath.Direction == CardinalDirection.None
                        || !(lowestPath.Direction != CardinalDirection.East && lowestPath.StepsThatDirection < minStepsInOneDirection)))
                {
                    var nextCell = charMap.GetCell(lowestPath.Cell.Coordinates.Row, lowestPath.Cell.Coordinates.Col + 1);
                    paths.Add(new HeatStep
                    {
                        Direction = CardinalDirection.East,
                        Cell = nextCell,
                        PreviousStep = lowestPath,
                        PreviousHeatLoss = lowestPath.TotalHeatLoss,
                        StepsThatDirection = lowestPath.Direction == CardinalDirection.East
                            ? lowestPath.StepsThatDirection + 1 : 1
                    });
                }
            }
        }
    }
}