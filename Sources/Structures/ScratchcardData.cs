namespace AoC2023.Structures
{
    public class ScratchcardData
    {
        public string Id { get; set; }
        public int[] WinningNumbers { get; set; }
        public int[] OwnedNumbers { get; set; }

        public override string ToString()
        {
            return $"Card {Id}: {string.Join(" ", WinningNumbers)} | {string.Join(" ", OwnedNumbers)}";
        }
    }
}