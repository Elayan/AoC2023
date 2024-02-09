using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoC2023.Structures;
using AoCTools.Loggers;
using AoCTools.Numbers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day05Almanac : WorkerBase
    {
        private AlmanacData _almanac;
        public override object Data => _almanac;

        public bool ReadAsSeedRanges { get; set; } = false;

        protected override void ProcessDataLines()
        {
            _almanac = new AlmanacData();
            ReadSeeds(DataLines[0]);

            var mapStarts = new List<int>();
            for (int i = 0; i < DataLines.Length; i++)
            {
                // start of a map is after a white line and a "x-to-y map:"
                if (string.IsNullOrWhiteSpace(DataLines[i]))
                    mapStarts.Add(i + 2);
            }

            _almanac.SeedToSoil = ReadPages(DataLines.Skip(mapStarts[0]).Take(mapStarts[1] - mapStarts[0] - 2).ToArray());
            _almanac.SoilToFertilizer = ReadPages(DataLines.Skip(mapStarts[1]).Take(mapStarts[2] - mapStarts[1] - 2).ToArray());
            _almanac.FertilizerToWater = ReadPages(DataLines.Skip(mapStarts[2]).Take(mapStarts[3] - mapStarts[2] - 2).ToArray());
            _almanac.WaterToLight = ReadPages(DataLines.Skip(mapStarts[3]).Take(mapStarts[4] - mapStarts[3] - 2).ToArray());
            _almanac.LightToTemperature = ReadPages(DataLines.Skip(mapStarts[4]).Take(mapStarts[5] - mapStarts[4] - 2).ToArray());
            _almanac.TemperatureToHumidity = ReadPages(DataLines.Skip(mapStarts[5]).Take(mapStarts[6] - mapStarts[5] - 2).ToArray());
            _almanac.HumidityToLocation = ReadPages(DataLines.Skip(mapStarts[6]).Take(DataLines.Length - mapStarts[6]).ToArray());
        }

        private void ReadSeeds(string line)
        {
            var split = line.Split(' ');
            var seeds = new List<AlmanacSeed>();
            var step = ReadAsSeedRanges ? 2 : 1;
            for (var i = 1; i < split.Length; i += step)
            {
                seeds.Add(!ReadAsSeedRanges
                    ? AlmanacSeed.CreateSeed(long.Parse(split[i]))
                    : AlmanacSeed.CreateSeed(long.Parse(split[i]), ulong.Parse(split[i + 1])));
            }
            _almanac.Seeds = seeds.ToArray();
        }

        private AlmanacPage[] ReadPages(string[] lines)
        {
            var pages = new List<AlmanacPage>();
            foreach (var line in lines)
            {
                var split = line.Split(' ');
                var to = long.Parse(split[0]);
                var from = long.Parse(split[1]);
                var range = ulong.Parse(split[2]);
                pages.Add(AlmanacPage.CreatePage(from, to, range));
            }
            return pages.ToArray();
        }

        protected override long WorkOneStar_Implementation()
        {
            var minLocation = -1L;
            foreach (var seed in _almanac.Seeds)
            {
                var location = GetLocationFromSeedId(seed.Id);
                if (minLocation == -1 || minLocation > location)
                {
                    minLocation = location;
                    Logger.Log("      (Updated minimal location)");
                }
            }

            Logger.Log($"Closest location = {minLocation}", SeverityLevel.Always);
            return minLocation;
        }

        private long GetLocationFromSeedId(long seedId)
        {
            var soil = GetDestinationFromPages(seedId, _almanac.SeedToSoil);
            var fertilizer = GetDestinationFromPages(soil, _almanac.SoilToFertilizer);
            var water = GetDestinationFromPages(fertilizer, _almanac.FertilizerToWater);
            var light = GetDestinationFromPages(water, _almanac.WaterToLight);
            var temperature = GetDestinationFromPages(light, _almanac.LightToTemperature);
            var humidity = GetDestinationFromPages(temperature, _almanac.TemperatureToHumidity);
            var location = GetDestinationFromPages(humidity, _almanac.HumidityToLocation);

            Logger.Log($"Seed {seedId} -> Soil {soil} -> Fertilizer {fertilizer} -> Water {water} -> Light {light} -> Temperature {temperature} -> Humidity {humidity} -> Location {location}");

            return location;
        }

        private long GetDestinationFromPages(long from, AlmanacPage[] pages)
        {
            foreach (var page in pages)
            {
                if (page.GetRanged(from, out long rangedTo))
                    return rangedTo;
            }
            return from;
        }

        protected override long WorkTwoStars_Implementation()
        {
            var minLocation = -1L;
            foreach (var seed in _almanac.Seeds)
            {
                var location = GetLowestLocationFromSeed(seed);
                if (minLocation == -1 || minLocation > location)
                {
                    minLocation = location;
                    Logger.Log("      (Updated minimal location)");
                }
            }

            Logger.Log($"Closest location = {minLocation}", SeverityLevel.Always);
            return minLocation;
        }

        private long GetLowestLocationFromSeed(AlmanacSeed seed)
        {
            var soil = GetDestinationsFromPages(new Range[] { seed }, _almanac.SeedToSoil);
            var fertilizer = GetDestinationsFromPages(soil, _almanac.SoilToFertilizer);
            var water = GetDestinationsFromPages(fertilizer, _almanac.FertilizerToWater);
            var light = GetDestinationsFromPages(water, _almanac.WaterToLight);
            var temperature = GetDestinationsFromPages(light, _almanac.LightToTemperature);
            var humidity = GetDestinationsFromPages(temperature, _almanac.TemperatureToHumidity);
            var location = GetDestinationsFromPages(humidity, _almanac.HumidityToLocation);

            if (Logger.ShowAboveSeverity == SeverityLevel.Never)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"[GetLocationFromSeed] {seed}");
                sb.AppendLine($"-> Soil {RangesToString(soil)}");
                sb.AppendLine($"-> Fertilizer {RangesToString(fertilizer)}");
                sb.AppendLine($"-> Water {RangesToString(water)}");
                sb.AppendLine($"-> Light {RangesToString(light)}");
                sb.AppendLine($"-> Temperature {RangesToString(temperature)}");
                sb.AppendLine($"-> Humidity {RangesToString(humidity)}");
                sb.AppendLine($"-> Location {RangesToString(location)}");
                Logger.Log(sb.ToString());
            }

            return location.Min(l => l.Min);
        }

        private string RangesToString(Range[] ranges)
        {
            return string.Join<Range>(" ; ", ranges);
        }

        private Range[] GetDestinationsFromPages(Range[] ranges, AlmanacPage[] pages)
        {
            var rangesToTransform = new List<Range>(ranges);
            var transformedRanges = new List<Range>();

            while (rangesToTransform.Count > 0)
            {
                var rangeToTransform = rangesToTransform.First();
                rangesToTransform.RemoveAt(0);

                var transformed = false;
                foreach (var page in pages)
                {
                    if (!page.GetRanged(rangeToTransform, out Range transformedRange, out Range[] leftOutRanges))
                        continue;

                    transformedRanges.Add(transformedRange);
                    rangesToTransform.AddRange(leftOutRanges);
                    transformed = true;
                }

                if (!transformed)
                {
                    // this won't move further
                    transformedRanges.Add(rangeToTransform);
                }
            }

            return transformedRanges.ToArray();
        }
    }
}