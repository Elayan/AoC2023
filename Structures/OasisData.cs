using System.Linq;
using System.Text;

namespace AoC2023.Structures
{
    public class OasisReport
    {
        public long[] Values { get; set; }

        public bool IsSimplifiable()
        {
            return Values.Any(v => v != 0);
        }

        public override string ToString()
        {
            return string.Join(" ", Values);
        }
    }
}