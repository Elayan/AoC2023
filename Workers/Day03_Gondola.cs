using System.Collections.Generic;
using System.Linq;
using AoC2023.Structures;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day03Gondola : WorkerBase
    {
        private const char IgnoreChar = '.';
        private const char GearChar = '*';
        private static readonly char[] NumberChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private GondolaEngineSchematic _schematic;

        protected override void ProcessDataLines()
        {
            _schematic = new GondolaEngineSchematic
            {
                RawSchematic = DataLines.Select(l => l.ToArray()).ToArray()
            };

            var numbers = new List<GondolaEngineNumber>();
            var symbols = new List<GondolaEngineSymbol>();
            for (var i = 0; i < DataLines.Length; i++)
            {
                var schematicLine = DataLines[i];
                for (var j = 0; j < schematicLine.Length; j++)
                {
                    var schematicChar = schematicLine[j];
                    if (schematicChar == IgnoreChar)
                        continue;

                    if (NumberChars.Contains(schematicChar))
                    {
                        ReadDigitSequence(schematicLine, j, out var digits, out int parsed);

                        var coords = new List<Coordinates>();
                        for (var x = 0; x < digits.Length; x++)
                        {
                            coords.Add(new Coordinates(i, j + x));
                        }

                        numbers.Add(new GondolaEngineNumber
                        {
                            Id = parsed,
                            Coords = coords.ToArray()
                        });

                        j += digits.Length - 1;
                    }
                    else
                    {
                        symbols.Add(new GondolaEngineSymbol
                        {
                            Id = $"{schematicChar}",
                            Coords = new Coordinates(i, j)
                        });
                    }
                }
            }

            _schematic.Numbers = numbers.ToArray();
            _schematic.Symbols = symbols.ToArray();
        }

        private static void ReadDigitSequence(string line, int startIndex, out char[] digits, out int parsed)
        {
            var digitList = new List<char>();
            var index = startIndex;
            parsed = 0;
            while (index < line.Length && NumberChars.Contains(line[index]))
            {
                digitList.Add(line[index]);
                parsed = parsed * 10 + int.Parse($"{line[index]}");
                index++;
            }
            digits = digitList.ToArray();
        }

        protected override long WorkOneStar_Implementation()
        {
            var sum = 0;
            foreach (var number in _schematic.Numbers)
            {
                if (CheckIfNumberHasSymbolNeighbor(number, _schematic.Symbols))
                {
                    Logger.Log($"{number.Id} is neighboring a symbol.");
                    sum += number.Id;
                }
            }

            Logger.Log($"Gondola engine value = {sum}", SeverityLevel.Always);
            return sum;
        }

        private static bool CheckIfNumberHasSymbolNeighbor(GondolaEngineNumber number, GondolaEngineSymbol[] symbols)
        {
            var neighborCoords = GetNeighborCoords(number.Coords);
            foreach (var symbol in symbols)
            {
                if (neighborCoords.Any(nc => nc.Equals(symbol.Coords)))
                    return true;
            }

            return false;
        }

        private static Coordinates[] GetNeighborCoords(Coordinates[] position)
        {
            var neighborCoords = new List<Coordinates>();

            // we're assuming number coords are always from left to right!
            for (var i = 0; i < position.Length; i++)
            {
                var coord = position[i];
                if (i == 0)
                {
                    neighborCoords.Add(new Coordinates(coord.Row - 1, coord.Col - 1));
                    neighborCoords.Add(new Coordinates(coord.Row, coord.Col - 1));
                    neighborCoords.Add(new Coordinates(coord.Row + 1, coord.Col - 1));
                }

                neighborCoords.Add(new Coordinates(coord.Row - 1, coord.Col));
                neighborCoords.Add(new Coordinates(coord.Row + 1, coord.Col));

                if (i == position.Length - 1)
                {
                    neighborCoords.Add(new Coordinates(coord.Row - 1, coord.Col + 1));
                    neighborCoords.Add(new Coordinates(coord.Row, coord.Col + 1));
                    neighborCoords.Add(new Coordinates(coord.Row + 1, coord.Col + 1));
                }
            }

            return neighborCoords.ToArray();
        }

        protected override long WorkTwoStars_Implementation()
        {
            var sum = 0L;
            foreach (var symbol in _schematic.Symbols)
            {
                if (symbol.Id[0] != GearChar)
                    continue;

                var numberNeighbors = FindNumberNeighborsForSymbol(symbol, _schematic.Numbers);
                Logger.Log($"At {symbol.Coords}, found {numberNeighbors.Length} neighbors.");

                if (numberNeighbors.Length != 2)
                {
                    Logger.Log("     (not a gear)");
                    continue;
                }

                var ratio = numberNeighbors[0].Id * numberNeighbors[1].Id;
                Logger.Log($"     (ratio = {ratio})");

                sum += ratio;
            }

            Logger.Log($"Gondola gear ratio = {sum}", SeverityLevel.Always);
            return sum;
        }

        private static GondolaEngineNumber[] FindNumberNeighborsForSymbol(GondolaEngineSymbol symbol, GondolaEngineNumber[] numbers)
        {
            var neighboringCoords = GetNeighborCoords(new[] { symbol.Coords });
            var neighborNumbers = new List<GondolaEngineNumber>();
            foreach (var number in numbers)
            {
                if (number.Coords.Any(c => neighboringCoords.Any(nc => nc.Equals(c))))
                    neighborNumbers.Add(number);
            }
            return neighborNumbers.ToArray();
        }
    }
}