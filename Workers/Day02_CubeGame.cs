using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AoC2023.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day02CubeGame : WorkerBase
    {
        private static readonly Regex CubeGameRoundRegex = new Regex("Game (?<id>[0-9]+)", RegexOptions.Compiled);
        private static readonly Regex CubeGameColorRegex = new Regex("(?<cnt>[0-9]+) (?<color>[a-z]+)", RegexOptions.Compiled);

        private CubeGameData _cubeGameData;

        public override object Data => _cubeGameData;
        protected override void ProcessDataLines()
        {
            var rounds = new List<CubeGameRoundData>();
            foreach (var line in DataLines)
            {
                var roundMatch = CubeGameRoundRegex.Match(line);

                var roundContentStr = line.Trim().Split(':')[1];
                var setsStr = roundContentStr.Split(';');
                var sets = new List<CubeGameSetData>();
                foreach (var setStr in setsStr)
                {
                    var colorsStr = setStr.Trim().Split(',');
                    var set = new CubeGameSetData();
                    foreach (var colorStr in colorsStr)
                    {
                        var colorMatch = CubeGameColorRegex.Match(colorStr);
                        var colorCount = int.Parse(colorMatch.Groups["cnt"].Value);
                        var colorName = colorMatch.Groups["color"].Value;

                        switch (colorName)
                        {
                            case "blue": set.Blues = colorCount; break;
                            case "green": set.Greens = colorCount; break;
                            case "red": set.Reds = colorCount; break;
                            default: throw new Exception($"[ReadAsCubeGame] Unknown color {colorName}!");
                        }
                    }
                    sets.Add(set);
                }

                rounds.Add(new CubeGameRoundData
                {
                    Id = int.Parse(roundMatch.Groups["id"].Value),
                    Sets = sets.ToArray()
                });
            }

            _cubeGameData = new CubeGameData
            {
                Rounds = rounds.ToArray()
            };
        }

        protected override long WorkOneStar_Implementation()
        {
            var sum = 0L;
            foreach (var round in _cubeGameData.Rounds)
            {
                if (round.MaxReds > 12 || round.MaxGreens > 13 || round.MaxBlues > 14)
                {
                    Logger.Log($"Round {round.Id} is invalid.");
                    continue;
                }

                sum += round.Id;
            }

            Logger.Log($"ANSWER = {sum}", SeverityLevel.Always);
            return sum;
        }

        protected override long WorkTwoStars_Implementation()
        {
            var sum = 0;
            foreach (var round in _cubeGameData.Rounds)
            {
                var power = round.MaxReds * round.MaxGreens * round.MaxBlues;
                Logger.Log($"Power of Round {round.Id} = {power}");
                sum += power;
            }

            Logger.Log($"ANSWER = {sum}", SeverityLevel.Always);
            return sum;
        }
    }
}