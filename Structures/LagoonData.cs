using System;
using System.Linq;
using AoCTools.Frame;
using AoCTools.Frame.Map;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Frame.TwoDimensions.Map;

namespace AoC2023.Structures
{
    public class LagoonDigInstruction
    {
        public LagoonDigInstruction(char direction, int length, string colorStr)
        {
            RawDirection = direction;
            Direction = CharToDirection(RawDirection);
            DirectionLength = length;
            RawColor = colorStr;
            ComputeColorInfos();
        }

        private CardinalDirection CharToDirection(char d)
        {
            switch (d)
            {
                case 'U': return CardinalDirection.North;
                case 'D': return CardinalDirection.South;
                case 'L': return CardinalDirection.West;
                case 'R': return CardinalDirection.East;
            }
            throw new Exception($"Unknown direction '{d}'");
        }

        private void ComputeColorInfos()
        {
            ColorDirectionLength = Convert.ToInt64(RawColor.Substring(0, 5), 16);
            switch (RawColor.Last())
            {
                case '0':
                    ColorDirection = CardinalDirection.East;
                    break;
                case '1':
                    ColorDirection = CardinalDirection.South;
                    break;
                case '2':
                    ColorDirection = CardinalDirection.West;
                    break;
                case '3':
                    ColorDirection = CardinalDirection.North;
                    break;
                default: throw new Exception($"Unknown direction {RawColor.Last()}");
            }
        }

        public char RawDirection { get; private set; }
        public CardinalDirection Direction { get; private set; }
        public int DirectionLength { get; private set; }
        public string RawColor { get; private set; }
        public CardinalDirection ColorDirection { get; private set; }
        public long ColorDirectionLength { get; private set; }

        public override string ToString()
        {
            return $"{Direction} {DirectionLength} - {RawColor}";
        }

        public string ToColorfulString()
        {
            return $"{ColorDirection} {ColorDirectionLength}";
        }
    }

    public class LagoonCharMap : CharMap
    {
        public LagoonCharMap(char[][] mapChars) : base(mapChars)
        {
        }
    }
}