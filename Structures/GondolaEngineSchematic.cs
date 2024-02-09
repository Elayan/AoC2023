using System.Linq;
using System.Text;
using AoCTools.Frame;
using AoCTools.Frame.TwoDimensions;

namespace AoC2023.Structures
{
    public class GondolaEngineSchematic
    {
        public char[][] RawSchematic { get; set; }
        public GondolaEngineSymbol[] Symbols { get; set; }
        public GondolaEngineNumber[] Numbers { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Symbols != null)
            {
                sb.AppendLine("=== SYMBOLS ===");
                foreach (var symbol in Symbols)
                {
                    sb.AppendLine($"- {symbol}");
                }
            }

            if (Numbers != null)
            {
                sb.AppendLine("=== NUMBERS ===");
                foreach (var number in Numbers)
                {
                    sb.AppendLine($"- {number}");
                }
            }

            sb.AppendLine("=== RAW SCHEMATIC ===");
            foreach (var rawSchematicLine in RawSchematic)
            {
                sb.AppendLine(string.Join("", rawSchematicLine));
            }

            return sb.ToString();
        }
    }

    public class GondolaEngineSymbol
    {
        public string Id { get; set; }
        public Coordinates Coords { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Coords}";
        }
    }

    public class GondolaEngineNumber
    {
        public int Id { get; set; }
        public Coordinates[] Coords { get; set; }

        public override string ToString()
        {
            return $"[{Id}] ({Coords[0].Row},{string.Join("-", Coords.Select(c => c.Col))})";
        }
    }
}