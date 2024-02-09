using System.Text.RegularExpressions;
using AoC2023.Structures;
using AoCTools.File;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day20Pulse : WorkerBase
    {
        private static readonly Regex ModuleRegex = new Regex(@"(?<type>%|&)?(?<name>[a-z]+) -> ((?<plug>[a-z]+)(, )?)+", RegexOptions.Compiled);

        private ModuleManager _moduleManager;
        public override object Data => _moduleManager;

        protected override void ProcessDataLines()
        {
            var manager = new ModuleManager();
            foreach (var line in DataLines)
            {
                var match = ModuleRegex.Match(line);

                var name = match.Groups["name"].Value;
                var type = match.Groups["type"].Success
                    ? match.Groups["type"].Value
                    : name;

                manager.AddModule(name, type);

                var plugGroup = match.Groups["plug"];
                if (plugGroup.Success)
                {
                    foreach (Capture capture in plugGroup.Captures)
                    {
                        if (string.IsNullOrEmpty(capture.Value))
                            continue;
                        manager.Plug(name, capture.Value);
                    }
                }
            }

            _moduleManager = manager;
        }

        protected override long WorkOneStar_Implementation()
        {
            var pulses = PressAThousand();
            Logger.Log($"Pulses = {pulses}", SeverityLevel.Always);
            return pulses;
        }

        private long PressAThousand()
        {
            var lows = 0L;
            var highs = 0L;
            for (var i = 0; i < 1000; i++)
            {
                _moduleManager.Pulse(i == 0, out var low, out var high, out _);
                lows += low;
                highs += high;
            }
            Logger.Log($"Low {lows} & high {highs}");
            return lows * highs;
        }

        protected override long WorkTwoStars_Implementation()
        {
            var rxMoved = false;
            var buttonPressCount = 0L;
            while (!rxMoved)
            {
                _moduleManager.Pulse(Logger.ShowAboveSeverity == SeverityLevel.Never,
                    out _, out _, out rxMoved);
                buttonPressCount++;
            }

            Logger.Log($"Button pressed {buttonPressCount} times to make RX move!", SeverityLevel.Always);
            return buttonPressCount;
        }
    }
}