using AoCTools.Strings;

namespace AoC2023.Structures
{
    public class BoatRaceData
    {
        public int Id { get; set; }
        public long Time { get; set; }
        public long WinningDistance { get; set; }

        public override string ToString()
        {
            var formatter = new StringFormatter();
            return $"{formatter.GetHumanFriendlyDistance(WinningDistance)}   in   {formatter.GetHumanFriendlyTime(Time)}";
        }
    }
}