using System.Text;
using Range = AoCTools.Numbers.Range;

namespace AoC2023.Structures
{
    public class AlmanacPage
    {
        private AlmanacPage() { }
        public static AlmanacPage CreatePage(long from, long to, ulong range)
        {
            return new AlmanacPage
            {
                From = Range.CreateFromRange(from, range),
                To = Range.CreateFromRange(to, range),
            };
        }

        public Range From { get; private set; }
        public Range To { get; private set; }

        public bool GetRanged(long from, out long rangedTo)
        {
            if (From.IsInRange(from))
            {
                rangedTo = To.GetRangedValue(From.GetRangeIndex(from));
                return true;
            }

            rangedTo = from;
            return false;
        }

        public bool GetRanged(Range range, out Range transformedRange, out Range[] leftOutRanges)
        {
            var transformed = false;
            if (From.IsInRange(range, out Range validRange))
            {
                transformedRange = To.GetRangedValues(From.GetRangeIndexes(validRange));
                leftOutRanges = Range.CreateFromExcluding(range, validRange);
                transformed = true;
            }
            else
            {
                transformedRange = null;
                leftOutRanges = new[] { range };
            }
            return transformed;
        }

        public override string ToString()
        {
            return $"{From} -> {To}";
        }
    }

    public class AlmanacSeed : Range
    {
        private AlmanacSeed() { }
        public static AlmanacSeed CreateSeed(long id)
        {
            return new AlmanacSeed
            {
                Min = id,
                Max = id,
                Size = 1
            };
        }
        public static AlmanacSeed CreateSeed(long id, ulong range)
        {
            return new AlmanacSeed
            {
                Min = id,
                Max = id + (long)range - 1,
                Size = range
            };
        }

        public long Id => Min;

        public override string ToString()
        {
            return $"Seed {base.ToString()}";
        }
    }

    public class AlmanacData
    {
        public AlmanacSeed[] Seeds { get; set; }
        public AlmanacPage[] SeedToSoil { get; set; }
        public AlmanacPage[] SoilToFertilizer { get; set; }
        public AlmanacPage[] FertilizerToWater { get; set; }
        public AlmanacPage[] WaterToLight { get; set; }
        public AlmanacPage[] LightToTemperature { get; set; }
        public AlmanacPage[] TemperatureToHumidity { get; set; }
        public AlmanacPage[] HumidityToLocation { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== === ALMANAC === ===");
            sb.AppendLine("=== Seeds ===");
            foreach (var seed in Seeds)
                sb.AppendLine(seed.ToString());
            sb.AppendLine();

            sb.AppendLine(PagesToString(SeedToSoil, "Seed to Soil"));
            sb.AppendLine(PagesToString(SoilToFertilizer, "Soil to Fertilizer"));
            sb.AppendLine(PagesToString(FertilizerToWater, "Fertilizer to Water"));
            sb.AppendLine(PagesToString(WaterToLight, "Water to Light"));
            sb.AppendLine(PagesToString(LightToTemperature, "Light to Temperature"));
            sb.AppendLine(PagesToString(TemperatureToHumidity, "Temperature to Humidity"));
            sb.AppendLine(PagesToString(HumidityToLocation, "Humidity to Location"));

            return sb.ToString();
        }
        private string PagesToString(AlmanacPage[] pages, string title)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== {title} ===");
            foreach (var page in pages)
                sb.AppendLine(page.ToString());
            return sb.ToString();
        }
    }
}