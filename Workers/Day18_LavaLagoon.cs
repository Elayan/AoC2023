using System;
using System.Linq;
using System.Text.RegularExpressions;
using AoC2023.Structures;
using AoCTools.File;
using AoCTools.Frame.Map.Extensions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day18LavaLagoon : WorkerBase
    {
        private static readonly Regex LagoonInstructionRegex =
            new Regex(@"(?<dir>[A-Z]) (?<len>[0-9]+) \(#(?<color>[0-9a-z]+)\)", RegexOptions.Compiled);

        private LagoonDigInstruction[] _instruction;
        public override object Data => _instruction;

        protected override void ProcessDataLines()
        {
            _instruction = DataLines.Select(l =>
            {
                var match = LagoonInstructionRegex.Match(l);
                return new LagoonDigInstruction(
                    match.Groups["dir"].Value[0],
                    int.Parse(match.Groups["len"].Value),
                    match.Groups["color"].Value);
            }).ToArray();
        }

        protected override long WorkOneStar_Implementation()
        {
            var area = ComputeTheLagoonArea(_instruction);
            Logger.Log($"Lagoon area = {area}", SeverityLevel.Always);
            return area;
        }

        private static long ComputeTheLagoonArea(LagoonDigInstruction[] instructions)
        {
            var startCoord = Coordinates.Zero;
            var previousCoord = startCoord;
            var perimeterSum = 0L;
            var areaSum = 0L;
            foreach (var instruction in instructions)
            {
                var nextCoord = previousCoord + instruction.DirectionLength * instruction.Direction.ToCoordinates();
                areaSum += previousCoord.Row * nextCoord.Col - previousCoord.Col * nextCoord.Row;
                perimeterSum += (long)Math.Sqrt(Math.Pow(nextCoord.Row - previousCoord.Row, 2) + Math.Pow(nextCoord.Col - previousCoord.Col, 2));
                Logger.Log($"Next coord at {nextCoord} - current area = {areaSum} || current perimeter = {perimeterSum}");
                previousCoord = nextCoord;
            }

            return Math.Abs(areaSum) / 2L + perimeterSum / 2L + 1;
        }

        protected override long WorkTwoStars_Implementation()
        {
            var area = ComputeTheColorfulLagoonArea(_instruction);
            Logger.Log($"Lagoon colorful area = {area}", SeverityLevel.Always);
            return area;
        }

        private static long ComputeTheColorfulLagoonArea(LagoonDigInstruction[] instructions)
        {
            var startCoord = Coordinates.Zero;
            var previousCoord = startCoord;
            var perimeterSum = 0L;
            var areaSum = 0L;
            foreach (var instruction in instructions)
            {
                var nextCoord = previousCoord + instruction.ColorDirectionLength * instruction.ColorDirection.ToCoordinates();
                areaSum += previousCoord.Row * nextCoord.Col - previousCoord.Col * nextCoord.Row;
                perimeterSum += (long)Math.Sqrt(Math.Pow(nextCoord.Row - previousCoord.Row, 2) + Math.Pow(nextCoord.Col - previousCoord.Col, 2));
                Logger.Log($"Next coord at {nextCoord} - current area = {areaSum} || current perimeter = {perimeterSum}");
                previousCoord = nextCoord;
            }

            return Math.Abs(areaSum) / 2L + perimeterSum / 2L + 1;
        }
    }
}