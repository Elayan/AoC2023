namespace AoC2023.Structures
{
    public class CalibrationStepArgument
    {
        public string CurrentLine { get; set; }

        public int LeftDigitIndex { get; set; }
        public int LeftDigitLength { get; set; }
        public int LeftDigitParsed { get; set; }

        public int RightDigitIndex { get; set; }
        public int RightDigitLength { get; set; }
        public int RightDigitParsed { get; set; }

        public int OldTotal { get; set; }
        public int NewTotal { get; set; }
    }
}