using System.Collections.Generic;
using System.Linq;
using AoC2023.Structures;
using AoCTools.Loggers;
using AoCTools.Strings;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day13MirrorValley : WorkerBase
    {
        private MirroredMap[] _mirrorValley;
        public override object Data => _mirrorValley;

        protected override void ProcessDataLines()
        {
            var lines = DataLines.ToList();
            var maps = new List<MirroredMap>();
            var mapIndex = 1;
            while (lines.Any())
            {
                var mapLines = lines.TakeWhile(l => !string.IsNullOrEmpty(l)).ToArray();
                maps.Add(new MirroredMap(mapLines, mapIndex++));
                lines = lines.Skip(mapLines.Length + 1).ToList();
            }

            _mirrorValley = maps.ToArray();
        }

        protected override long WorkOneStar_Implementation()
        {
            var mirroredUpRows = 0;
            var mirroredLeftColumns = 0;
            foreach (var map in _mirrorValley)
            {
                Logger.Log($"Inspecting map {map.Id}", SeverityLevel.High);

                var foundMirror = false;

                var sameLines = Enumerable.Range(0, map.Lines.Length - 1)
                    .Where(r => map.Lines[r] == map.Lines[r + 1]).ToList();
                if (sameLines.Any())
                {
                    foreach (var sameLine in sameLines)
                    {
                        Logger.Log($"Two same lines at {sameLine}", SeverityLevel.Medium);

                        if (map.IsHorizontalMirror(sameLine))
                        {
                            Logger.Log($"Horizontal Mirror at {sameLine}-{sameLine+1}", SeverityLevel.High);
                            mirroredUpRows += sameLine + 1;
                            foundMirror = true;
                        }
                    }
                }

                var sameCols = Enumerable.Range(0, map.Columns.Length - 1)
                    .Where(r => map.Columns[r] == map.Columns[r + 1]).ToList();
                if (sameCols.Any())
                {
                    foreach (var sameCol in sameCols)
                    {
                        Logger.Log($"Two same columns at {sameCol}", SeverityLevel.Medium);

                        if (map.IsVerticalMirror(sameCol))
                        {
                            Logger.Log($"Vertical Mirror at {sameCol}-{sameCol+1}", SeverityLevel.High);
                            mirroredLeftColumns += sameCol + 1;
                            foundMirror = true;
                        }
                    }
                }

                if (!foundMirror)
                    Logger.Log($"NO MIRROR FOUND FOR MAP {map.Id}", SeverityLevel.High);
            }

            var total = mirroredUpRows * 100 + mirroredLeftColumns;
            Logger.Log($"Found the mirrors = {total}", SeverityLevel.Always);
            return total;
        }

        protected override long WorkTwoStars_Implementation()
        {
            var mirroredUpRows = 0;
            var mirroredLeftColumns = 0;
            foreach (var map in _mirrorValley)
            {
                Logger.Log($"Inspecting map {map.Id}", SeverityLevel.High);

                var foundMirror = false;

                var sameLines = Enumerable.Range(0, map.Lines.Length - 1)
                    .Where(r => map.Lines[r].CountDifferences(map.Lines[r + 1]) <= 1).ToList();
                if (sameLines.Any())
                {
                    foreach (var sameLine in sameLines)
                    {
                        Logger.Log($"Two (almost?) same lines at {sameLine}", SeverityLevel.Medium);

                        if (map.IsHorizontalMirror(sameLine, true))
                        {
                            Logger.Log($"Horizontal Smudged Mirror at {sameLine}-{sameLine+1}", SeverityLevel.High);
                            mirroredUpRows += sameLine + 1;
                            foundMirror = true;
                            break;
                        }
                    }
                }

                if (foundMirror)
                    continue;

                var sameCols = Enumerable.Range(0, map.Columns.Length - 1)
                    .Where(r => map.Columns[r].CountDifferences(map.Columns[r + 1]) <= 1).ToList();
                if (sameCols.Any())
                {
                    foreach (var sameCol in sameCols)
                    {
                        Logger.Log($"Two (almost?) same columns at {sameCol}", SeverityLevel.Medium);

                        if (map.IsVerticalMirror(sameCol, true))
                        {
                            Logger.Log($"Vertical Smudged Mirror at {sameCol}-{sameCol+1}", SeverityLevel.High);
                            mirroredLeftColumns += sameCol + 1;
                            foundMirror = true;
                            break;
                        }
                    }
                }

                if (!foundMirror)
                    Logger.Log($"NO MIRROR FOUND FOR MAP {map.Id}", SeverityLevel.High);
            }

            var total = mirroredUpRows * 100 + mirroredLeftColumns;
            Logger.Log($"Found the smudged mirrors = {total}", SeverityLevel.Always);
            return total;
        }
    }
}