using System;
using System.Linq;
using System.Text;

namespace AoC2023.Structures
{
    public class CubeGameData
    {
        public CubeGameRoundData[] Rounds { get; set; }

        public override string ToString()
        {
            return string.Join<CubeGameRoundData>(Environment.NewLine, Rounds);
        }
    }

    public class CubeGameRoundData
    {
        public int Id { get; set; }
        public CubeGameSetData[] Sets { get; set; }

        public int MaxReds => Sets.Max(s => s.Reds);
        public int MaxGreens => Sets.Max(s => s.Greens);
        public int MaxBlues => Sets.Max(s => s.Blues);

        public override string ToString()
        {
            return $"Round {Id}: {string.Join<CubeGameSetData>(" ; ", Sets)}";
        }
    }

    public class CubeGameSetData
    {
        public int Reds { get; set; }
        public int Blues { get; set; }
        public int Greens { get; set; }

        public override string ToString()
        {
            return $"red = {Reds}, blue = {Blues}, green = {Greens}";
        }
    }
}