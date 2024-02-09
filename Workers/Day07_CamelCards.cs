using System.Collections.Generic;
using System.Linq;
using AoC2023.Structures;
using AoCTools.File;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day07CamelCards : WorkerBase
    {
        private CamelCardHand[] _hands;
        public override object Data => _hands;

        public bool UseJokers { get; set; } = false;

        protected override void ProcessDataLines()
        {
            var hands = new List<CamelCardHand>();
            foreach (var line in DataLines)
            {
                var split = line.Split(' ');
                var cards = new List<CamelCard>();
                foreach (var c in split[0])
                {
                    if (UseJokers)
                        cards.Add(JokeCamelCard.CreateCard($"{c}"));
                    else cards.Add(NoJokeCamelCard.CreateCard($"{c}"));
                }
                hands.Add(CamelCardHand.CreateHand(cards.ToArray(), int.Parse(split[1]), UseJokers));
            }
            _hands = hands.ToArray();
        }

        protected override long WorkOneStar_Implementation()
        {
            return ComputeWinnings();
        }

        protected override long WorkTwoStars_Implementation()
        {
            return ComputeWinnings();
        }

        private long ComputeWinnings()
        {
            var orderedHands = _hands.OrderBy(h => h).ToList();
            if (Logger.ShowAboveSeverity == SeverityLevel.Never)
            {
                Logger.Log("=== ORDERED ===", SeverityLevel.Medium);
                foreach (var hand in orderedHands)
                    Logger.Log(hand.ToString(), SeverityLevel.Medium);
            }

            var total = orderedHands.Select((t, i) => (i + 1) * t.Bid).Sum();

            Logger.Log($"Winnings ({(UseJokers ? "with" : "without")} jokers) = {total}", SeverityLevel.Always);
            return total;
        }
    }
}