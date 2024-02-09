using System.Text;
using AoCTools.Frame.Map;
using AoCTools.Frame.TwoDimensions.Map;
using AoCTools.Strings;

namespace AoC2023.Structures
{
    public class MirroredMap : CrossLineMap
    {
        public MirroredMap(string[] lines, int id)
            : base(lines)
        {
            Id = id;
        }

        public int Id { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== MAP {Id} ===");
            sb.AppendLine(base.ToString());
            return sb.ToString();
        }

        public bool IsHorizontalMirror(int index, bool smudged = false)
        {
            return IsMirror(index, Lines, smudged);
        }

        public bool IsVerticalMirror(int index, bool smudged = false)
        {
            return IsMirror(index, Columns, smudged);
        }

        private bool IsMirror(int index, string[] array, bool smudged)
        {
            var reflectionIndex = index + 1;
            var allowedErrors = smudged ? 1 : 0;
            var totalErrors = 0;
            while (index >= 0 && reflectionIndex < array.Length)
            {
                totalErrors += array[index].CountDifferences(array[reflectionIndex]);
                if (totalErrors > allowedErrors)
                    return false;
                index--;
                reflectionIndex++;
            }
            return totalErrors == allowedErrors;
        }
    }
}