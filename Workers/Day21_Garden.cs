using System;
using System.Collections.Generic;
using System.Linq;
using AoC2023.Structures;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day21Garden : WorkerBase
    {
        private GardenMap _map;
        public override object Data => _map;

        protected override void ProcessDataLines()
        {
            _map = new GardenMap(DataLines.Select(l => l.ToArray()).ToArray());
        }

        public int Steps { get; set; }

        protected override long WorkOneStar_Implementation()
        {
            return HowManyPlotsToSee(false);
        }

        protected override long WorkTwoStars_Implementation()
        {
            return HowManyPlotsToSee(true);
        }

        private long HowManyPlotsToSee(bool infiniteMap)
        {
            WalkAndCount(_map, Steps, infiniteMap);

            if (Logger.ShowAboveSeverity == SeverityLevel.Never)
                Logger.Log(_map.ToDistanceString());

            long standingPlots;
            if (infiniteMap)
            {
                standingPlots = _map.AllPlots.Sum(c =>
                {
                    var validDistances = c.Distances.Where(d =>
                        d.Distance != -1 && d.Distance <= Steps && d.Distance % 2 == Steps % 2).ToList();
                    var validCount = validDistances.Sum(vd => vd.Extension.GetComputingPower());
                    var validDistStr = string.Join(", ", validDistances.Select(vd => vd.ToString()));
                    var allDistStr = string.Join(", ", c.Distances.Select(ad => ad.ToString()));
                    Logger.Log($"Cell {c.Coordinates} has {validCount} valid distances\nValid = {validDistStr}\nAll = {allDistStr}");
                    return validCount;
                });
            }
            else
            {
                standingPlots = _map.AllPlots.Count(c =>
                {
                    var distance = c.GetDistanceForExtension(GardenExtension.Central);
                    return distance != -1
                           && distance <= Steps
                           && distance % 2 == Steps % 2;
                });
            }

            Logger.Log($"Walking {Steps} steps in the garden allows to stop on {standingPlots} different plots.", SeverityLevel.Always);
            return standingPlots;
        }

        private static void WalkAndCount(GardenMap map, int maxSteps, bool infiniteMap)
        {
            var paths = new List<GardenStep>
            {
                new GardenStep
                {
                    Cell = map.StartingPoint,
                    Extension = GardenExtension.Central,
                    Steps = -1
                }
            };

            while (paths.Any())
            {
                var step = paths[0];
                paths.RemoveAt(0);

                var distance = step.Steps + 1;
                step.Cell.SetDistanceForExtension(step.Extension, distance);
                if (distance == maxSteps)
                    continue; // don't walk more than necessary

                var allNeighboringPlots = GetUnvisitedNeighboringPlots(step, map, infiniteMap);
                var neighborsToAdd = allNeighboringPlots
                    .Where(s =>
                        s.Item1.GetDistanceForExtension(s.Item2) == -1
                        && !paths.Any(p => p.Cell == s.Item1 && p.Extension.Equals(s.Item2)))
                    .Select(c => new GardenStep
                    {
                        Cell = c.Item1,
                        Steps = distance,
                        Extension = c.Item2
                    })
                    .ToList();
                paths.AddRange(neighborsToAdd);
            }
        }

        private static Tuple<GardenCell, GardenExtension>[] GetUnvisitedNeighboringPlots(GardenStep step, GardenMap map, bool infiniteMap)
        {
            var cell = step.Cell;
            var plots = new List<Tuple<GardenCell, GardenExtension>>();

            if (cell.Coordinates.Row > 0 || infiniteMap && step.Extension.CanExtend(CardinalDirection.North))
            {
                var extension = step.Extension.GetCopy();
                var row = cell.Coordinates.Row - 1;
                if (row < 0)
                {
                    row = map.RowCount - 1;
                    extension.Extend(CardinalDirection.North);
                }

                var neighbor = map.MapCells[row][cell.Coordinates.Col];
                if (neighbor.CellType == GardenCellType.Plot)
                    plots.Add(new Tuple<GardenCell, GardenExtension>(neighbor, extension));
            }

            if (cell.Coordinates.Row < map.RowCount - 1 || infiniteMap && step.Extension.CanExtend(CardinalDirection.South))
            {
                var extension = step.Extension.GetCopy();
                var row = cell.Coordinates.Row + 1;
                if (row >= map.RowCount)
                {
                    row = 0;
                    extension.Extend(CardinalDirection.South);
                }

                var neighbor = map.MapCells[row][cell.Coordinates.Col];
                if (neighbor.CellType == GardenCellType.Plot)
                    plots.Add(new Tuple<GardenCell, GardenExtension>(neighbor, extension));
            }

            if (cell.Coordinates.Col > 0 || infiniteMap && step.Extension.CanExtend(CardinalDirection.West))
            {
                var extension = step.Extension.GetCopy();
                var col = cell.Coordinates.Col - 1;
                if (col < 0)
                {
                    col = map.ColCount - 1;
                    extension.Extend(CardinalDirection.West);
                }

                var neighbor = map.MapCells[cell.Coordinates.Row][col];
                if (neighbor.CellType == GardenCellType.Plot)
                    plots.Add(new Tuple<GardenCell, GardenExtension>(neighbor, extension));
            }

            if (cell.Coordinates.Col < map.ColCount - 1 || infiniteMap && step.Extension.CanExtend(CardinalDirection.East))
            {
                var extension = step.Extension.GetCopy();
                var col = cell.Coordinates.Col + 1;
                if (col >= map.ColCount)
                {
                    col = 0;
                    extension.Extend(CardinalDirection.East);
                }

                var neighbor = map.MapCells[cell.Coordinates.Row][col];
                if (neighbor.CellType == GardenCellType.Plot)
                    plots.Add(new Tuple<GardenCell, GardenExtension>(neighbor, extension));
            }

            return plots.ToArray();
        }
    }
}