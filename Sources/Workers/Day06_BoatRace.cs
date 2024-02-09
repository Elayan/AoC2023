using System.Collections.Generic;
using System.Linq;
using AoC2023.Structures;
using AoCTools.Loggers;
using AoCTools.Strings;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day06BoatRace : WorkerBase
    {
        private BoatRaceData[] _boatRaces;
        public override object Data => _boatRaces;

        public bool ReadWithoutSpaces { get; set; }

        protected override void ProcessDataLines()
        {
            var timesLine = string.Join("", DataLines[0].Skip(5));
            if (ReadWithoutSpaces)
            {
                timesLine = timesLine.Replace(" ", "");
            }

            var times = new List<long>();
            foreach (var timeStr in timesLine.Split(' '))
            {
                if (string.IsNullOrWhiteSpace(timeStr))
                    continue;

                times.Add(long.Parse(timeStr));
            }

            var distancesLine = string.Join("", DataLines[1].Skip(9));
            if (ReadWithoutSpaces)
            {
                distancesLine = distancesLine.Replace(" ", "");
            }

            var distances = new List<long>();
            foreach (var distanceStr in distancesLine.Split(' '))
            {
                if (string.IsNullOrWhiteSpace(distanceStr))
                    continue;

                distances.Add(long.Parse(distanceStr));
            }

            var races = new List<BoatRaceData>();
            for (int i = 0; i < times.Count; i++)
            {
                races.Add(new BoatRaceData
                {
                    Id = i + 1,
                    Time = times[i],
                    WinningDistance = distances[i]
                });
            }

            _boatRaces = races.ToArray();
        }

        protected override long WorkOneStar_Implementation()
        {
            return ComputeWaysToWin();
        }

        protected override long WorkTwoStars_Implementation()
        {
            return ComputeWaysToWin();
        }

        private long ComputeWaysToWin()
        {
            var formatter = new StringFormatter();

            var mult = 0;
            foreach (var race in _boatRaces)
            {
                var holdDuration = 0;
                var waysToWin = 0;
                var bestDistance = -1L;
                var bestHolding = -1L;
                while (true)
                {
                    holdDuration++;
                    var timeLeftAfterHold = race.Time - holdDuration;
                    if (timeLeftAfterHold <= 0)
                    {
                        Logger.Log($"Holding the entire race ({formatter.GetHumanFriendlyTime(holdDuration)}) can't win.");
                        break;
                    }

                    var distanceTraveled = holdDuration * timeLeftAfterHold;
                    if (bestDistance == -1 || distanceTraveled > bestDistance)
                    {
                        bestDistance = distanceTraveled;
                        bestHolding = holdDuration;
                    }

                    if (distanceTraveled > race.WinningDistance)
                    {
                        Logger.Log($"Can win by holding {formatter.GetHumanFriendlyTime(holdDuration)} ({formatter.GetHumanFriendlyDistance(distanceTraveled)} traveled)");
                        waysToWin++;
                    }
                    else
                    {
                        Logger.Log($"No win by holding {formatter.GetHumanFriendlyTime(holdDuration)} ({formatter.GetHumanFriendlyDistance(distanceTraveled)} < {formatter.GetHumanFriendlyDistance(race.WinningDistance)})");
                    }
                }

                Logger.Log($"Best score: hold {formatter.GetHumanFriendlyTime(bestHolding)} to travel {formatter.GetHumanFriendlyDistance(bestDistance)}", SeverityLevel.High);

                mult = mult == 0 ? waysToWin : mult * waysToWin;
            }

            Logger.Log($"Ways to win = {mult}", SeverityLevel.Always);
            return mult;
        }
    }
}