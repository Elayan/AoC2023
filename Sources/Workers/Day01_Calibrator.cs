using System.Text.RegularExpressions;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day01Calibrator : WorkerBase
    {
        private static readonly Regex OnlyNumbersRegex = new Regex("(?=([0-9]))", RegexOptions.Compiled);
        private static readonly Regex AllNumbersRegex =
            new Regex("(?=([0-9]|zero|one|two|three|four|five|six|seven|eight|nine))", RegexOptions.Compiled);

        protected override void ProcessDataLines()
        {
            // nothing to do.
        }

        protected override long WorkOneStar_Implementation()
        {
            return DecipherCalibrationValues(OnlyNumbersRegex, int.Parse);
        }

        protected override long WorkTwoStars_Implementation()
        {
            return DecipherCalibrationValues(AllNumbersRegex, ParseAllNumbers);
        }

        private long DecipherCalibrationValues(Regex digitFinderRegex, Func<string, int> digitParser)
        {
            long sum = 0;
            for (var i = 0; i < DataLines.Length; i++)
            {
                var line = DataLines[i];
                var matches = digitFinderRegex.Matches(line);
                Logger.Log($"Found {matches.Count} digits in line {i}: {line}.");

                var firstMatch = matches[0].Groups[1];
                var lastMatch = matches[matches.Count - 1].Groups[1];
                Logger.Log($"Digits as strings: '{firstMatch.Value}' and '{lastMatch.Value}'");

                var firstParsed = digitParser.Invoke(firstMatch.Value);
                var lastParsed = digitParser.Invoke(lastMatch.Value);
                Logger.Log($"Digits as parsed values: {firstParsed} and {lastParsed}");

                var combo = 10 * firstParsed + lastParsed;
                Logger.Log($"Combo is {combo}");

                sum += combo;
            }

            Logger.Log($"Sum of all Calibration Values = {sum}", SeverityLevel.Always);
            return sum;
        }

        private static int ParseAllNumbers(string number)
        {
            switch (number)
            {
                case "zero": return 0;
                case "one": return 1;
                case "two": return 2;
                case "three": return 3;
                case "four": return 4;
                case "five": return 5;
                case "six": return 6;
                case "seven": return 7;
                case "eight" : return 8;
                case "nine" : return 9;
                default: return int.Parse(number);
            }
        }
    }
}