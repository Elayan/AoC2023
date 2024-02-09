using System.Linq;
using AoC2023.Structures;
using AoCTools.Loggers;
using AoCTools.Numbers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day14RockyBalance : WorkerBase
    {
        private RockyPlate _plate;
        public override object Data => _plate;

        protected override void ProcessDataLines()
        {
            _plate = new RockyPlate(DataLines);
        }

        protected override long WorkOneStar_Implementation()
        {
            long totalWeight = 0L;
            for (var i = 0; i < _plate.Columns.Length; i++)
            {
                Logger.Log($"Column {i} - rocks {string.Join(", ", _plate.RocksOnColumns[i])}");

                long colWeight = 0L;
                for (var c = 0; c < _plate.RocksOnColumns[i].Length - 1; c++)
                {
                    var begin = _plate.RocksOnColumns[i][c];
                    var end = _plate.RocksOnColumns[i][c + 1] - 1;
                    var ballCount =
                        Enumerable.Range(begin, end - begin)
                            .Count(r => _plate.Columns[i][r] == 'O');
                    var weight = ComputeWeight(begin, ballCount, _plate.ColumnsSize);
                    Logger.Log($"Computing for the {ballCount} balls between {begin} and {end} = {weight}");
                    colWeight += weight;
                }

                Logger.Log($"Weight = {colWeight}");
                totalWeight += colWeight;
            }

            Logger.Log($"Total north weight = {totalWeight}", SeverityLevel.Always);
            return totalWeight;
        }

        private static long ComputeWeight(int ballsIndex, int ballCount, int lineSize)
        {
            return NonoMath.SumFirstIntegers(lineSize - ballsIndex)
                   - NonoMath.SumFirstIntegers(lineSize - ballsIndex - ballCount);
        }
    }
}