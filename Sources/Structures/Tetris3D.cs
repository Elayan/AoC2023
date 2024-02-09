using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoCTools.Frame.ThreeDimensions;

namespace AoC2023.Structures
{
    public class Tetris3D
    {
        public Tetris3D(TetrisBlock[] blocks)
        {
            Blocks = blocks;

            ComputeSizes();
        }

        public TetrisBlock[] Blocks { get; }

        private void ComputeSizes()
        {
            MinRow = Blocks.Min(b => Math.Min(b.Start.Row, b.End.Row));
            MaxRow = Blocks.Max(b => Math.Max(b.Start.Row, b.End.Row));
            RowSize = MaxRow - MinRow + 1;

            MinCol = Blocks.Min(b => Math.Min(b.Start.Col, b.End.Col));
            MaxCol = Blocks.Max(b => Math.Max(b.Start.Col, b.End.Col));
            ColSize = MaxCol - MinCol + 1;

            Height = Blocks.Max(b => Math.Max(b.Start.Height, b.End.Height)) + 1;
        }

        public long MinRow { get; private set; }
        public long MaxRow { get; private set; }
        public long RowSize { get; private set; }
        public long MinCol { get; private set; }
        public long MaxCol { get; private set; }
        public long ColSize { get; private set; }
        public long Height { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("=== TETRIS ===");
            foreach (var block in Blocks)
            {
                sb.Append(Environment.NewLine);
                sb.Append(block);
            }
            return sb.ToString();
        }

        public TetrisBlockBit[] BlocksAsBitsFromSideX()
        {
            return BlocksAsBitsFromSide(true);
        }

        public TetrisBlockBit[] BlocksAsBitsFromSideY()
        {
            return BlocksAsBitsFromSide(false);
        }

        /// <param name="xSide">TRUE if X-side, FALSE if Y-side</param>
        private TetrisBlockBit[] BlocksAsBitsFromSide(bool xSide)
        {
            var allBits = new List<TetrisBlockBit>();
            foreach (var block in Blocks)
            {
                if (block.Orientation == TetrisBlockOrientation.Vertical)
                {
                    var lowH = Math.Min(block.Start.Height, block.End.Height);
                    var highH = Math.Max(block.Start.Height, block.End.Height);
                    for (var h = lowH; h <= highH; h++)
                    {
                        allBits.Add(new TetrisBlockBit(h,
                            block.Name,
                            xSide ? block.Start.Row : block.Start.Col,
                            xSide ? block.Start.Col : block.Start.Row));
                    }
                }
                else
                {
                    var lowerSide = xSide
                        ? Math.Min(block.Start.Row, block.End.Row)
                        : Math.Min(block.Start.Col, block.End.Col);
                    var higherSide = xSide
                        ? Math.Max(block.Start.Row, block.End.Row)
                        : Math.Max(block.Start.Col, block.End.Col);

                    var closerToSide = xSide
                        ? Math.Min(block.Start.Col, block.End.Col)
                        : Math.Min(block.Start.Row, block.End.Row);

                    for (var s = lowerSide; s <= higherSide; s++)
                    {
                        allBits.Add(new TetrisBlockBit(block.Start.Height,
                            block.Name, s, closerToSide));
                    }
                }
            }

            for (var i = 0; i < allBits.Count; i++)
            {
                var curBit = allBits[i];
                var otherBits = allBits.Skip(i).Where(b => b.Height == curBit.Height && b.X == curBit.X).ToArray();
                var closerOtherY = otherBits.Min(b => b.Y);
                if (curBit.Y > closerOtherY)
                {
                    allBits.Remove(curBit);
                }

                foreach (var otherBit in otherBits)
                {
                    if (otherBit.Y == closerOtherY)
                        continue;

                    allBits.Remove(otherBit);
                }
            }

            return allBits.ToArray();
        }

        public void DropBlocks()
        {
            var orderedBlocks = Blocks.OrderBy(b => b.Start.Height).ToList();
            var droppedBlocks = new List<TetrisBlock>();
            while (orderedBlocks.Any())
            {
                var toDrop = orderedBlocks.First();
                orderedBlocks.Remove(toDrop);

                var underBlocks = droppedBlocks
                    .Where(db => toDrop.HaveCommonShadowWith(db))
                    .ToList();
                if (underBlocks.Any())
                {
                    var higherUnderHeight = underBlocks.Max(u => u.End.Height);
                    toDrop.SetHeightTo(higherUnderHeight + 1);

                    var supportingBlocks = underBlocks.Where(u => u.End.Height == higherUnderHeight).ToList();
                    toDrop.SupportingBlocks.AddRange(supportingBlocks);
                    foreach (var supportingBlock in supportingBlocks)
                        supportingBlock.SupportedBlocks.Add(toDrop);
                }
                else
                {
                    toDrop.SetHeightTo(1);
                }

                droppedBlocks.Add(toDrop);
            }
        }
    }

    public enum TetrisBlockOrientation
    {
        Vertical,
        AlongRow,
        AlongCol
    }

    public class TetrisBlock
    {
        public TetrisBlock(Coordinates start, Coordinates end, string name)
        {
            Name = name;

            if (start.Height <= end.Height
                || start.Row < end.Height
                || start.Col < end.Col)
            {
                Start = start;
                End = end;
            }
            else
            {
                End = start;
                Start = end;
            }

            if (Start.Height != End.Height)
            {
                Orientation = TetrisBlockOrientation.Vertical;
                BlockLength = End.Height - Start.Height + 1;
            }
            else if (Start.Row != End.Row)
            {
                Orientation = TetrisBlockOrientation.AlongRow;
                BlockLength = End.Row - Start.Row + 1;
            }
            else
            {
                Orientation = TetrisBlockOrientation.AlongCol;
                BlockLength = End.Col - Start.Col + 1;
            }
        }

        public string Name { get; }
        public Coordinates Start { get; private set; }
        public Coordinates End { get; private set; }

        public TetrisBlockOrientation Orientation { get; }

        public long BlockLength { get; }

        public List<TetrisBlock> SupportingBlocks { get; } = new List<TetrisBlock>();
        public List<TetrisBlock> SupportedBlocks { get; } = new List<TetrisBlock>();

        public override string ToString()
        {
            return $"[{Name}] {Start} to {End}";
        }

        public void SetHeightTo(long h)
        {
            Start = new Coordinates(Start.Row, Start.Col, h);
            End = Orientation == TetrisBlockOrientation.Vertical
                ? new Coordinates(End.Row, End.Col, h + BlockLength - 1)
                : new Coordinates(End.Row, End.Col, h);
        }

        public bool HaveCommonShadowWith(TetrisBlock db)
        {
            return Start.Row <= db.End.Row
                   && End.Row >= db.Start.Row
                   && Start.Col <= db.End.Col
                   && End.Col >= db.Start.Col;
        }

        public long GetCountOfBlocksWhoWouldFallWithoutIt()
        {
            // get all supported blocks
            var allSupportedBlocks = new List<TetrisBlock>();
            GetAllSupportedBlocks(allSupportedBlocks);
            allSupportedBlocks = allSupportedBlocks.Distinct().ToList();

            var willSurvive = allSupportedBlocks
                .Where(sb
                    => sb.SupportingBlocks.Any(ssb
                        => ssb != this && !allSupportedBlocks.Contains(ssb)))
                .ToList();
            while (willSurvive.Any())
            {
                allSupportedBlocks = allSupportedBlocks.Except(willSurvive).ToList();
                willSurvive = allSupportedBlocks
                    .Where(sb
                        => sb.SupportingBlocks.Any(ssb
                            => ssb != this && !allSupportedBlocks.Contains(ssb)))
                    .ToList();
            }

            return allSupportedBlocks.Count;
        }

        private void GetAllSupportedBlocks(List<TetrisBlock> supportedBlocks)
        {
            supportedBlocks.AddRange(SupportedBlocks);
            foreach (var supportedBlock in SupportedBlocks)
                supportedBlock.GetAllSupportedBlocks(supportedBlocks);
        }
    }

    public class TetrisBlockBit
    {
        public TetrisBlockBit(long height, string name, long x, long y)
        {
            Height = height;
            Name = name;
            X = x;
            Y = y;
        }

        public long Height { get; }
        public string Name { get; }
        public long X { get; }
        public long Y { get; }
    }
}