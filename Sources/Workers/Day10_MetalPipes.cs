using System.Collections.Generic;
using System.Linq;
using AoC2023.Structures;
using AoCTools.File;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day10MetalPipes : WorkerBase
    {
        private MetalMap _map;
        public override object Data => _map;

        protected override void ProcessDataLines()
        {
            _map = new MetalMap(DataLines.Select(l => l.ToArray()).ToArray());
        }

        protected override long WorkOneStar_Implementation()
        {
            WalkThePipe(_map.StartingPoint);
            var dist = _map.GetGreatestDistance();
            Logger.Log($"Greatest distance is {dist}", SeverityLevel.Always);
            return dist;
        }

        private void WalkThePipe(MetalCell startingPoint)
        {
            startingPoint.DistanceFromStart = 0;
            var paths = new List<MetalCell> { startingPoint };
            var visitedCells = new List<MetalCell>();
            while (paths.Any())
            {
                var cell = paths[0];
                paths.RemoveAt(0);

                visitedCells.Add(cell);
                cell.IsPipe = true;
                foreach (var neighbor in cell.Neighbors)
                {
                    if (!visitedCells.Contains(neighbor))
                    {
                        neighbor.DistanceFromStart = cell.DistanceFromStart + 1;
                        paths.Add(neighbor);
                    }
                }
            }
        }

        protected override long WorkTwoStars_Implementation()
        {
            WalkThePipe(_map.StartingPoint);

            var pipeCells = _map.AllCells.Where(c => c.IsPipe).ToList();
            Logger.Log($"Pipe is {pipeCells.Count} cells long.");

            foreach (var cell in _map.AllCells)
            {
                if (cell.IsPipe)
                    continue;

                // going to the right, counting how many limits we cross
                // we count a piece of continuous (horizontal) pipe as ONE crossed pipe
                // if the pipe is just a quick hook (F7 or LJ) it doesn't count at all
                var crossedPipes = 0;
                var loopStartedF = false;
                var loopStartedL = false;
                for (var j = cell.Coordinates.Col + 1; j < _map.MapCells[0].Length; j++)
                {
                    var inspectedCell = _map.MapCells[cell.Coordinates.Row][j];
                    if (!inspectedCell.IsPipe)
                        continue; // not a pipe

                    if (inspectedCell.IsEastPipe)
                    {
                        if (inspectedCell.GroundType == MetalGroundType.NorthEastPipe)
                            loopStartedL = true;
                        else if (inspectedCell.GroundType == MetalGroundType.SouthEastPipe)
                            loopStartedF = true;

                        continue; // continuing pipe
                    }

                    if (loopStartedF)
                    {
                        loopStartedF = false;
                        if (inspectedCell.GroundType == MetalGroundType.SouthWestPipe)
                            continue; // ignore brushing loop
                    }
                    else if (loopStartedL)
                    {
                        loopStartedL = false;
                        if (inspectedCell.GroundType == MetalGroundType.NorthWestPipe)
                            continue; // ignore brushing loop
                    }

                    crossedPipes++;
                }

                if (crossedPipes % 2 == 1)
                    cell.IsNest = true;
            }
            Logger.Log(_map.ToString());

            var nests = _map.AllCells.Count(c => c.IsNest);
            Logger.Log($"We found {nests} nests!", SeverityLevel.Always);
            return nests;
        }
    }
}