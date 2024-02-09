namespace AoC2023.Structures
{
    public class Lens
    {
        public string Name { get; set; }
        public int Box { get; set; }
        public int Focal { get; set; }

        public long GetPower(int slot) => (Box + 1) * (slot + 1) * Focal;
    }
}