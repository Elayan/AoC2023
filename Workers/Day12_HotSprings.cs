using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoC2023.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day12HotSprings : WorkerBase
    {
        private SemiNonogram _nonogram;
        public override object Data => _nonogram;

        protected override void ProcessDataLines()
        {
            _nonogram = new SemiNonogram();
            foreach (var line in DataLines)
            {
                var split = line.Split(' ');
                _nonogram.Lines.Add(new NonogramLine(
                    split[0],
                    split[1].Split(',').Select(int.Parse).ToArray()));
            }
        }

        protected override long WorkOneStar_Implementation()
        {
            return SolveTheSemiNonogram(false);
        }

        protected override long WorkTwoStars_Implementation()
        {
            return SolveTheSemiNonogram(true);
        }

        private long SolveTheSemiNonogram(bool unfold)
        {
            if (unfold)
            {
                _nonogram.Unfold();
                Logger.Log(_nonogram.ToString());
            }

            var sum = 0;
            foreach (var nonoLine in _nonogram.Lines)
            {
                Logger.Log($"Computing combinations for {nonoLine}");
                var combinations = CountCombinations(nonoLine, unfold);
                Logger.Log($"Found {combinations} possibilities.");
                sum += combinations;
            }

            Logger.Log($"There are {sum} total possibilities!", SeverityLevel.Always);
            return sum;
        }

        private static int CountCombinations(NonogramLine nonoLine, bool unfold)
        {
            // get all blocks possible placements
            var possiblePlacementsForBlocks = new Dictionary<int,List<int>>(); // < block index ; indexes it can be placed
            for (var i = 0; i < nonoLine.Serie.Length; i++)
            {
                possiblePlacementsForBlocks.Add(i,
                    nonoLine.FillableIndexes
                        .Where(f => nonoLine.Serie[i] <= f.Value // block length can fit
                                                    && (f.Key == 0
                                                        || !nonoLine.FilledIndexes.Contains(f.Key - 1)) // at the beginning of the line, or previous slot can be empty
                                                    && (f.Key + nonoLine.Serie[i] == nonoLine.Size
                                                        || !nonoLine.FilledIndexes.Contains(f.Key + nonoLine.Serie[i]))) // at the end of the line, or next slot can be empty
                        .Select(f => f.Key)
                        .ToList());
            }

            var blockPlacements = Enumerable.Repeat(-1, nonoLine.Serie.Length).ToArray();
            return PlaceBlocksAndCount(blockPlacements, possiblePlacementsForBlocks, nonoLine, unfold);
        }

        private static int PlaceBlocksAndCount(int[] inBlockPlacements, Dictionary<int,List<int>> inPossiblePlacements, NonogramLine nonoLine, bool unfold)
        {
            var possiblePlacements = inPossiblePlacements.ToDictionary(p => p.Key, p => p.Value.ToList());
            var blockPlacements = inBlockPlacements.ToArray();

            // place all with only one choice
            while (possiblePlacements.Any(p => p.Value.Count <= 1))
            {
                if (possiblePlacements.Any(p => p.Value.Count == 0))
                    return 0; // can't place this block anywhere, impossible combination

                var onlyOnePlace = possiblePlacements.First(p => p.Value.Count == 1).Key;
                blockPlacements[onlyOnePlace] = possiblePlacements[onlyOnePlace][0];
                possiblePlacements.Remove(onlyOnePlace);
                FilterPossiblePlacements(
                    possiblePlacements,
                    onlyOnePlace,
                    blockPlacements[onlyOnePlace],
                    nonoLine);
            }

            // nothing more to place, we're a good one!
            if (possiblePlacements.Count == 0)
            {
                var dicoPlacement = Enumerable.Range(0, blockPlacements.Length).ToDictionary(r => blockPlacements[r], r => nonoLine.Serie[r]);
                if (!nonoLine.Validate(dicoPlacement))
                    return 0;

                if (!unfold)
                    LogBlocks(dicoPlacement, nonoLine.Size);
                return 1;
            }

            var fewestOptionCount = possiblePlacements.Min(p => p.Value.Count);
            var fewOptionsIndex = possiblePlacements.First(p => p.Value.Count == fewestOptionCount).Key;
            var fewOptionPlaces = possiblePlacements[fewOptionsIndex];
            possiblePlacements.Remove(fewOptionsIndex);

            var validOptions = 0;
            foreach (var place in fewOptionPlaces)
            {
                blockPlacements[fewOptionsIndex] = place;

                var copyPossiblePlacements = possiblePlacements.ToDictionary(p => p.Key, p => p.Value.ToList());
                FilterPossiblePlacements(
                    copyPossiblePlacements,
                    fewOptionsIndex,
                    place,
                    nonoLine);

                if (copyPossiblePlacements.Any(p => p.Value.Count == 0))
                    continue; // can't place this block anywhere, impossible combination

                validOptions += PlaceBlocksAndCount(blockPlacements, copyPossiblePlacements, nonoLine, unfold);
            }

            return validOptions;
        }

        private static void FilterPossiblePlacements(Dictionary<int,List<int>> possiblePlacements, int blockIndex, int beginIndex, NonogramLine nonoLine)
        {
            var endIndex = beginIndex + nonoLine.Serie[blockIndex];
            foreach (var possiblePlacement in possiblePlacements)
            {
                var possibleBlockLength = nonoLine.Serie[possiblePlacement.Key];
                possiblePlacement.Value.RemoveAll(possibleBeginIndex =>
                {
                    var possibleEndIndex = possibleBeginIndex + possibleBlockLength;
                    return
                        possiblePlacement.Key < blockIndex && possibleBeginIndex >= beginIndex           // previous block should begin before
                            || possiblePlacement.Key > blockIndex && possibleBeginIndex <= endIndex    // following block should begin after
                            || beginIndex <= possibleBeginIndex && possibleBeginIndex <= endIndex       // begins in the middle of the placed block
                            || beginIndex <= possibleEndIndex && possibleEndIndex <= endIndex;          // ends in the middle of the placed block
                });
            }
        }

        private static int CountCombinations_OLD(NonogramLine nonoLine, bool unfold)
        {
            var blocks = new Dictionary<int, int>(); // < start index ; length >

            // first setup
            int nextBlock = -1;
            foreach (var num in nonoLine.Serie)
            {
                nextBlock = nonoLine.FillableIndexes.First(f => nextBlock < f.Key && num <= f.Value).Key;
                blocks.Add(nextBlock, num);
                nextBlock += num;
            }

            var validCount = 0;
            var blockIndexToDecal = blocks.Keys.Max();
            while (blockIndexToDecal >= 0)
            {
                // validate and count if valid
                if (nonoLine.Validate(blocks))
                {
                    if (!unfold)
                        LogBlocks(blocks, nonoLine.Size);
                    validCount++;
                }

                blockIndexToDecal = DecalBlockAt(blocks, blockIndexToDecal, nonoLine, unfold);
            }

            return validCount;
        }

        private static int DecalBlockAt(Dictionary<int, int> blocks, int blockToDecal, NonogramLine nonoLine, bool unfold)
        {
            // find next spot to decal to
            var nextDecalSpot = -1;
            while (true)
            {
                try
                {
                    nextDecalSpot = nonoLine.FillableIndexes.First(f => blockToDecal < f.Key && blocks[blockToDecal] <= f.Value).Key;
                    if (nextDecalSpot + blocks[blockToDecal] <= nonoLine.Size
                        && !blocks.Keys.Any(k => k != blockToDecal && k <= nextDecalSpot + blocks[blockToDecal] && nextDecalSpot < k + blocks[k]))
                        break;

                    // can't decal more without overflowing, let's decal the previous one
                    try
                    {
                        blockToDecal = blocks.Keys.OrderBy(b => b).Last(b => b < blockToDecal);
                    }
                    catch
                    {
                        // no more block to decal
                        return -1;
                    }
                }
                catch
                {
                    // no next decal spot, let's decal the previous one
                    try
                    {
                        blockToDecal = blocks.Keys.OrderBy(b => b).Last(b => b < blockToDecal);
                    }
                    catch
                    {
                        // no more block to decal
                        return -1;
                    }
                }
            }

            var allAfter = blocks
                .Where(b => b.Key > blockToDecal)
                .Select(b => b.Key)
                .OrderBy(k => k)
                .ToList();

            // decal to next spot
            var blockToDecalLength = blocks[blockToDecal];
            blocks.Remove(blockToDecal);
            blocks.Add(nextDecalSpot, blockToDecalLength);

            // set all followings right after
            var pos = nextDecalSpot;
            foreach (var after in allAfter)
            {
                pos = nonoLine.FillableIndexes.FirstOrDefault(f => pos + blockToDecalLength < f.Key && blocks[after] <= f.Value).Key;
                blockToDecalLength = blocks[after];
                if (pos == after)
                    continue; // already at the correct spot

                blocks.Remove(after);
                blocks.Add(pos, blockToDecalLength);
            }

            if (!unfold) Logger.Log(string.Join("-", blocks.Keys.OrderBy(b => b)));

            return blocks.Keys.Max();
        }

        private static void LogBlocks(Dictionary<int, int> blocks, int size)
        {
            if (Logger.ShowAboveSeverity != SeverityLevel.Never)
                return;

            var sb = new StringBuilder();
            var pos = 0;
            foreach (var block in blocks)
            {
                var spacing = block.Key - pos;
                sb.Append(string.Join("", Enumerable.Repeat('.', spacing)));
                pos += spacing;
                sb.Append(string.Join("", Enumerable.Repeat('#', block.Value)));
                pos += block.Value;
            }

            sb.Append(string.Join("", Enumerable.Repeat('.', size - pos)));

            Logger.Log(sb.ToString());
        }
    }
}