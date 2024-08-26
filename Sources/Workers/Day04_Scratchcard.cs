using System.Text.RegularExpressions;
using AoC2023.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day04Scratchcard : WorkerBase
    {
        private ScratchcardData[] _scratchcards;
        public override object Data => _scratchcards;

        protected override void ProcessDataLines()
        {
            var scratchCards = new List<ScratchcardData>();
            foreach (var line in DataLines)
            {
                var split = line.Split(':');
                var id = split[0].Split(' ')[1];

                var numberStrings = split[1].Trim().Split('|');

                var winningStrings = numberStrings[0].Trim().Split(' ');
                var winnings = new List<int>();
                foreach (var winningString in winningStrings)
                    if (!string.IsNullOrWhiteSpace(winningString))
                        winnings.Add(int.Parse(winningString));

                var ownedStrings = numberStrings[1].Trim().Split(' ');
                var owns = new List<int>();
                foreach (var ownedString in ownedStrings)
                    if (!string.IsNullOrWhiteSpace(ownedString))
                        owns.Add(int.Parse(ownedString));

                scratchCards.Add(new ScratchcardData
                {
                    Id = id,
                    WinningNumbers = winnings.ToArray(),
                    OwnedNumbers = owns.ToArray()
                });
            }

            _scratchcards = scratchCards.ToArray();
        }

        protected override long WorkOneStar_Implementation()
        {
            var sum = 0L;
            foreach (var card in _scratchcards)
            {
                var actualWinningCount = GetActualWinningsCount(card);
                if (actualWinningCount == 0)
                {
                    Logger.Log($"Card {card.Id} is worth nothing.");
                    continue;
                }

                var score = (int)Math.Pow(2, actualWinningCount - 1);
                Logger.Log($"Card {card.Id} is winning {actualWinningCount} times. Score = {score}.");

                sum += score;
            }

            Logger.Log($"Simple score = {sum}", SeverityLevel.Always);
            return sum;
        }

        private static int GetActualWinningsCount(ScratchcardData card)
        {
            return card.OwnedNumbers.Count(o => card.WinningNumbers.Contains(o));
        }

        protected override long WorkTwoStars_Implementation()
        {
            var cardCounts = new int[_scratchcards.Length];
            for (int i = 0; i < cardCounts.Length; i++) cardCounts[i] = 1;

            for (int i = 0; i < _scratchcards.Length; i++)
            {
                var winCount = GetActualWinningsCount(_scratchcards[i]);
                Logger.Log($"The {cardCounts[i]} copies of Card {_scratchcards[i].Id} makes you win {winCount} each.");

                // add to the [win] next cards an extra copy for each copy of the current card
                while (winCount > 0)
                {
                    cardCounts[i + winCount] += cardCounts[i];
                    winCount--;
                }

                if (Logger.ShowAboveSeverity == SeverityLevel.Never)
                {
                    Logger.Log($"{string.Join(" ", _scratchcards.Zip(cardCounts, (c, cnt) => $"{c.Id}:{cnt}"))}");
                }
            }

            var sum = cardCounts.Sum();
            Logger.Log($"Complex score = {sum}", SeverityLevel.Always);
            return sum;
        }
    }
}