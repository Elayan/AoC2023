using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoC2023.Structures;
using AoCTools.Frame.ThreeDimensions;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day22SandTetris : WorkerBase
    {
        private Tetris3D _tetris;
        public override object Data => _tetris;

        protected override void ProcessDataLines()
        {
            var idx = 0;
            _tetris = new Tetris3D(DataLines.Select(l =>
            {
                var half = l.Split('~');
                var partsA = half[0].Split(',');
                var partsB = half[1].Split(',');
                idx++;
                return new TetrisBlock(
                    new Coordinates(
                        long.Parse(partsA[0]),
                        long.Parse(partsA[1]),
                        long.Parse(partsA[2])),
                    new Coordinates(
                        long.Parse(partsB[0]),
                        long.Parse(partsB[1]),
                        long.Parse(partsB[2])),
                    idx.ToString());
            }).ToArray());
        }

        protected override long WorkOneStar_Implementation()
        {
            ShowTheTower();

            _tetris.DropBlocks();

            ShowTheTower();

            var canBeDestroyedCount = 0;
            foreach (var block in _tetris.Blocks)
            {
                if (block.SupportedBlocks.Count == 0
                    || block.SupportedBlocks.All(sb => sb.SupportingBlocks.Count > 1))
                {
                    canBeDestroyedCount++;
                }
            }

            Logger.Log($"{canBeDestroyedCount} blocks can be disintegrated!", SeverityLevel.Always);
            return canBeDestroyedCount;
        }

        protected override long WorkTwoStars_Implementation()
        {
            ShowTheTower();

            _tetris.DropBlocks();

            ShowTheTower();

            var canBeSafelyDestroyed = new List<TetrisBlock>();
            foreach (var block in _tetris.Blocks)
            {
                if (block.SupportedBlocks.Count == 0
                    || block.SupportedBlocks.All(sb => sb.SupportingBlocks.Count > 1))
                {
                    canBeSafelyDestroyed.Add(block);
                }
            }
            Logger.Log($"{canBeSafelyDestroyed.Count} blocks can be safely destroyed ({_tetris.Blocks.Length - canBeSafelyDestroyed.Count} to destroy).", SeverityLevel.High);

            var unsafeToDestroy = _tetris.Blocks.Except(canBeSafelyDestroyed);
            var totalFalls = 0L;
            foreach (var block in unsafeToDestroy)
            {
                var totalSupported = block.GetCountOfBlocksWhoWouldFallWithoutIt();
                totalFalls += totalSupported;
                Logger.Log($"Block {block.Name} supports {totalSupported} blocks (total = {totalFalls})");
            }

            Logger.Log($"{totalFalls} blocks would be falling!", SeverityLevel.Always);
            return totalFalls;
        }

        private void ShowTheTower()
        {
            if (Logger.ShowAboveSeverity != SeverityLevel.Never)
                return;

            ShowSide(
                _tetris.BlocksAsBitsFromSideX(),
                _tetris.RowSize, _tetris.Height, "x");
            ShowSide(
                _tetris.BlocksAsBitsFromSideY(),
                _tetris.ColSize, _tetris.Height, "y");
        }

        private void ShowSide(TetrisBlockBit[] blocksFromSideWithDepth, long sideSize, long height, string label)
        {
            var tab = new char[height][];
            for (var x = 0; x < tab.Length; x++)
                tab[x] = Enumerable.Repeat('.', (int)sideSize).ToArray();

            foreach (var block in blocksFromSideWithDepth)
            {
                tab[block.Height][block.X] = block.Name[0];
            }

            var sb = new StringBuilder();
            sb.AppendLine($"-- {label} -->");
            for (var h = height - 1; h >= 0; h--)
            {
                sb.Append($"[{h}] ");
                sb.AppendLine(string.Join(" ", tab[h]));
            }

            Logger.Log(sb.ToString());
        }
    }
}