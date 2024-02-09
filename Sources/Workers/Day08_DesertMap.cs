using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AoC2023.Structures;
using AoCTools.File;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day08DesertMap : WorkerBase
    {
        private static readonly Regex MapPointRegex = new Regex(@"(?<point>[A-Z0-9]+) = \((?<left>[A-Z0-9]+), (?<right>[A-Z0-9]+)\)", RegexOptions.Compiled);

        private DesertMapData _map;
        public override object Data => _map;

        protected override void ProcessDataLines()
        {
            _map = new DesertMapData();
            foreach (var direction in DataLines[0])
            {
                _map.AddDirection(direction);
            }

            foreach (var line in DataLines.Skip(2))
            {
                var match = MapPointRegex.Match(line);
                _map.AddPoint(
                    match.Groups["point"].Value,
                    match.Groups["left"].Value,
                    match.Groups["right"].Value);
            }
        }

        protected override long WorkOneStar_Implementation()
        {
            var currentPoint = _map.StartingPoint;
            var sb = new StringBuilder(currentPoint.Name);

            var step = 0;
            while (currentPoint.Name != "ZZZ")
            {
                var direction = _map.GetDirection(step++);
                currentPoint = direction == DesertMapDirection.Left
                    ? currentPoint.LeftPoint
                    : currentPoint.RightPoint;

                if (Logger.ShowAboveSeverity == SeverityLevel.Never)
                    sb.Append($" -> {currentPoint.Name}");
            }

            if (Logger.ShowAboveSeverity == SeverityLevel.Never)
                Logger.Log(sb.ToString());

            Logger.Log($"Destination in {step} steps!", SeverityLevel.Always);
            return step;
        }

        protected override long WorkTwoStars_Implementation()
        {
            var infos = new List<SimplifiedGhostPath>();
            foreach (var ghostStartingPoint in _map.GhostStartingPoints)
                infos.Add(GetSimplified(ghostStartingPoint, _map.Directions));

            var nextArrivals = new long[infos.Count];
            for (int i = 0; i < infos.Count; i++)
                nextArrivals[i] = infos[i].GetNextArrival();

            var step = nextArrivals.Min();
            Logger.Log($"Starting at earliest {step}", SeverityLevel.Medium);

            while (true)
            {
                var arrivalCount = 0;
                for (int i = 0; i < infos.Count; i++)
                {
                    if (nextArrivals[i] != step)
                        continue;

                    nextArrivals[i] = infos[i].GetNextArrival();
                    arrivalCount++;
                }

                if (arrivalCount == infos.Count)
                {
                    Logger.Log($"Ghost steps = {step}", SeverityLevel.Always);
                    return step;
                }

                step = nextArrivals.Min();
                Logger.Log($"Continuing with {step}");
            }
        }

        private SimplifiedGhostPath GetSimplified(DesertMapPoint startPoint, DesertMapDirection[] directions)
        {
            var currentPoint = startPoint;
            long steps = 0;

            var possibleLoops = new Dictionary<long, long>(); // < startStep, indexInSequence >
            while (true)
            {
                var stepInSequence = steps % directions.Length;
                var direction = directions[stepInSequence];
                currentPoint = direction == DesertMapDirection.Left
                    ? currentPoint.LeftPoint
                    : currentPoint.RightPoint;
                steps++;

                if (currentPoint.Name.EndsWith("Z"))
                {
                    possibleLoops.Add(steps, stepInSequence);

                    var loop = possibleLoops.FirstOrDefault(l => l.Key != steps && l.Value == stepInSequence);
                    if (loop.Key != 0)
                    {
                        var path = new SimplifiedGhostPath(possibleLoops.Keys.ToArray());
                        Logger.Log($"Found loop through {string.Join(", ", path.AllFirstArrivals)}", SeverityLevel.Medium);
                        return path;
                    }
                }
            }
        }
    }
}