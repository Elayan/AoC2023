using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC2023.Structures
{
    public enum CamelCardStrength
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    public class CamelCardHand : IComparable<CamelCardHand>
    {
        private CamelCardHand() { }
        public static CamelCardHand CreateHand(CamelCard[] cards, int bid, bool useJokers)
        {
            var hand = new CamelCardHand
            {
                Cards = cards,
                Bid = bid
            };
            hand.Initialize(useJokers);
            return hand;
        }

        public CamelCard[] Cards { get; private set; }
        public int Bid { get; private set; }

        public CamelCardStrength Strength { get; private set; }

        private void Initialize(bool useJokers)
        {
            var cardByFigure = new Dictionary<string, List<CamelCard>>();
            foreach (var card in Cards)
            {
                if (cardByFigure.ContainsKey(card.FigureString))
                    cardByFigure[card.FigureString].Add(card);
                else cardByFigure.Add(card.FigureString, new List<CamelCard>{ card });
            }

            if (cardByFigure.Any(cbf => cbf.Value.Count == 5))
            {
                Strength = CamelCardStrength.FiveOfAKind;
                return;
            }

            var jokerCount = Cards.Count(c => c.FigureValue == 1);
            var fourCount = cardByFigure.FirstOrDefault(cbf => cbf.Value.Count == 4);
            if (!string.IsNullOrEmpty(fourCount.Key))
            {
                if (useJokers && jokerCount > 0) // we have a Joker to add to our Four, or we have four Jokers to turn into our 5th figure
                    Strength = CamelCardStrength.FiveOfAKind;
                else Strength = CamelCardStrength.FourOfAKind;
                return;
            }

            var threeCount = cardByFigure.FirstOrDefault(cbf => cbf.Value.Count == 3);
            var twoCounts = cardByFigure.Where(cbf => cbf.Value.Count == 2).ToArray();
            if (!string.IsNullOrEmpty(threeCount.Key))
            {
                if (twoCounts.Length == 1)
                {
                    if (useJokers && jokerCount > 0) // we have jokers, in the Pair or the Three, but in both cases it forms a whole Five
                        Strength = CamelCardStrength.FiveOfAKind;
                    else Strength = CamelCardStrength.FullHouse;
                }
                else
                {
                    if (useJokers && jokerCount > 0) // we have only one joker (otherwise it would be a Pair) to add to our Three, or our Three are Jokers that will turn into something else
                        Strength = CamelCardStrength.FourOfAKind;
                    else Strength = CamelCardStrength.ThreeOfAKind;
                }
                return;
            }

            if (twoCounts.Length == 2)
            {
                if (useJokers && (twoCounts[0].Key == "J" || twoCounts[1].Key == "J")) // one of our Pairs is made of Jokers, let's form a Four instead
                    Strength = CamelCardStrength.FourOfAKind;
                else if (useJokers && Cards.Any(c => c.FigureValue == 1)) // we have a Joker that we can add to one of our Pairs to form a Full
                    Strength = CamelCardStrength.FullHouse;
                else Strength = CamelCardStrength.TwoPair;
                return;
            }

            if (twoCounts.Length == 1)
            {
                if (useJokers && jokerCount > 0) // we have an extra Joker to make a Three, or our Pair is made of Jokers that turn into something else
                    Strength = CamelCardStrength.ThreeOfAKind;
                else Strength = CamelCardStrength.OnePair;
                return;
            }

            if (useJokers && jokerCount > 0)
                Strength = CamelCardStrength.OnePair;
            else Strength = CamelCardStrength.HighCard;
        }

        public override string ToString()
        {
            return $"Hand {string.Join("", Cards.Select(c => c.FigureString))} - {Bid} - {Strength}";
        }

        public int CompareTo(CamelCardHand other)
        {
            if (Strength != other.Strength)
                return Strength.CompareTo(other.Strength);

            for (int i = 0; i < 5; i++)
                if (Cards[i].FigureValue != other.Cards[i].FigureValue)
                    return Cards[i].FigureValue.CompareTo(other.Cards[i].FigureValue);

            return 0;
        }
    }

    public abstract class CamelCard
    {
        protected CamelCard() { }

        public int FigureValue { get; protected set; }
        public string FigureString { get; protected set; }
    }

    public class NoJokeCamelCard : CamelCard
    {
        public static NoJokeCamelCard CreateCard(string figure)
        {
            var card = new NoJokeCamelCard { FigureString = figure };
            if (int.TryParse(figure, out var parsed))
                card.FigureValue = parsed;
            else
            {
                switch (figure)
                {
                    case "T": card.FigureValue = 10; break;
                    case "J": card.FigureValue = 11; break;
                    case "Q": card.FigureValue = 12; break;
                    case "K": card.FigureValue = 13; break;
                    case "A": card.FigureValue = 14; break;
                }
            }
            return card;
        }
    }

    public class JokeCamelCard : CamelCard
    {
        public static JokeCamelCard CreateCard(string figure)
        {
            var card = new JokeCamelCard { FigureString = figure };
            if (int.TryParse(figure, out var parsed))
                card.FigureValue = parsed;
            else
            {
                switch (figure)
                {
                    case "J": card.FigureValue = 1; break;

                    case "T": card.FigureValue = 10; break;
                    case "Q": card.FigureValue = 11; break;
                    case "K": card.FigureValue = 12; break;
                    case "A": card.FigureValue = 13; break;
                }
            }
            return card;
        }
    }
}