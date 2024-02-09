using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AoC2023.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day15Manual : WorkerBase
    {
        protected override void ProcessDataLines()
        {
            // nothing to do.
        }

        protected override long WorkOneStar_Implementation()
        {
            var result = DataLines[0]
                .Split(',')
                .Select(ComputeWordValue)
                .Sum();

            Logger.Log($"Interpret the text = {result}", SeverityLevel.Always);
            return result;
        }

        private static long ComputeWordValue(string word)
        {
            var total = 0L;
            foreach (var c in word)
            {
                total += c;
                total *= 17L;
                total %= 256;
            }
            return total;
        }

        private static readonly Regex LensStep = new Regex(@"(?<name>[a-z]+)(-|=(?<focal>[0-9]+))", RegexOptions.Compiled);

        protected override long WorkTwoStars_Implementation()
        {
            var steps = DataLines[0].Split(',');

            var lenses = new List<Lens>();
            var boxes = new Dictionary<int, List<Lens>>(); // < box index, slotted lenses >
            foreach (var step in steps)
            {
                var match = LensStep.Match(step);
                var name = match.Groups["name"].Value;
                var isFocal = match.Groups["focal"].Success;
                if (!isFocal)
                {
                    var namedLens = lenses.FirstOrDefault(l => l.Name == name);
                    if (namedLens != null)
                    {
                        boxes[namedLens.Box].Remove(namedLens);
                        lenses.Remove(namedLens);
                    }
                }
                else
                {
                    var placeInBox = (int)ComputeWordValue(name);
                    var lens = lenses.FirstOrDefault(l => l.Box == placeInBox && l.Name == name);
                    if (lens != null)
                    {
                        lens.Focal = int.Parse(match.Groups["focal"].Value);
                        continue;
                    }

                    lens = new Lens
                    {
                        Name = name,
                        Box = placeInBox,
                        Focal = int.Parse(match.Groups["focal"].Value)
                    };
                    lenses.Add(lens);

                    if (!boxes.ContainsKey(lens.Box))
                        boxes.Add(lens.Box, new List<Lens>(0));
                    boxes[lens.Box].Add(lens);
                }
            }

            var answer = lenses.Sum(l => l.GetPower(boxes[l.Box].FindIndex(s => s == l)));
            Logger.Log($"Setup lenses = {answer}");
            return answer;
        }
    }
}