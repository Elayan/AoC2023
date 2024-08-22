using AoC2023.Workers;
using AoCTools.Loggers;
using AoCTools.Workers;
using NUnit.Framework;
using System.IO;

namespace AoC2023_Exec
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class Samples
    {
        private string GetSamplePath(int day, string append = "")
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Samples", $"sample{day:00}{append}");
        }

        private void TestOneStar(IWorker worker, string dataPath, long expectedResult)
        {
            var work = worker.WorkOneStar(dataPath, SeverityLevel.Never);
            Assert.That(work, Is.EqualTo(expectedResult), $"One Star returned {work}, expected {expectedResult}.");
        }

        private void TestTwoStars(IWorker worker, string dataPath, long expectedResult)
        {
            var work = worker.WorkTwoStars(dataPath, SeverityLevel.Never);
            Assert.That(work, Is.EqualTo(expectedResult), $"Two Stars returned {work}, expected {expectedResult}.");
        }

        [Test]
        public void Day01()
        {
            TestOneStar(new Day01Calibrator(), GetSamplePath(1), 142);
            TestTwoStars(new Day01Calibrator(), GetSamplePath(1, "b"), 281);
        }

        [Test]
        public void Day02()
        {
            TestOneStar(new Day02CubeGame(), GetSamplePath(2), 8);
            TestTwoStars(new Day02CubeGame(), GetSamplePath(2), 2286);
        }

        [Test]
        public void Day03()
        {
            TestOneStar(new Day03Gondola(), GetSamplePath(3), 4361);
            TestTwoStars(new Day03Gondola(), GetSamplePath(3), 467835);
        }

        [Test]
        public void Day04()
        {
            TestOneStar(new Day04Scratchcard(), GetSamplePath(4), 13);
            TestTwoStars(new Day04Scratchcard(), GetSamplePath(4), 30);
        }

        [Test]
        public void Day05()
        {
            TestOneStar(new Day05Almanac(), GetSamplePath(5), 35);
            TestTwoStars(new Day05Almanac { ReadAsSeedRanges = true }, GetSamplePath(5), 46);
        }

        [Test]
        public void Day06()
        {
            TestOneStar(new Day06BoatRace(), GetSamplePath(6), 288);
            TestTwoStars(new Day06BoatRace { ReadWithoutSpaces = true }, GetSamplePath(6), 71503);
        }

        [Test]
        public void Day07()
        {
            TestOneStar(new Day07CamelCards(), GetSamplePath(7), 6440);
            TestTwoStars(new Day07CamelCards { UseJokers = true }, GetSamplePath(7), 5905);
        }

        [Test]
        public void Day07_AllHands()
        {
            TestOneStar(new Day07CamelCards(), GetSamplePath(7, "b"), 351);
            TestTwoStars(new Day07CamelCards { UseJokers = true }, GetSamplePath(7, "b"), 351);
        }

        [Test]
        public void Day08()
        {
            TestOneStar(new Day08DesertMap(), GetSamplePath(8, "a"), 2);
            TestOneStar(new Day08DesertMap(), GetSamplePath(8, "b"), 6);
            TestTwoStars(new Day08DesertMap(), GetSamplePath(8, "c"), 6);
        }

        [Test]
        public void Day09()
        {
            TestOneStar(new Day09OasisStability(), GetSamplePath(9), 114);
            TestTwoStars(new Day09OasisStability(), GetSamplePath(9), 2);
        }

        [Test]
        public void Day10()
        {
            TestOneStar(new Day10MetalPipes(), GetSamplePath(10, "a"), 4);
            TestOneStar(new Day10MetalPipes(), GetSamplePath(10, "b"), 8);
            TestTwoStars(new Day10MetalPipes(), GetSamplePath(10, "c"), 4);
            TestTwoStars(new Day10MetalPipes(), GetSamplePath(10, "d"), 8);
        }

        [Test]
        public void Day11()
        {
            TestOneStar(new Day11Galaxy(), GetSamplePath(11), 374);
            TestTwoStars(new Day11Galaxy { ExpansionRate = 9 }, GetSamplePath(11), 1030);
            TestTwoStars(new Day11Galaxy { ExpansionRate = 99 }, GetSamplePath(11), 8410);
            TestTwoStars(new Day11Galaxy { ExpansionRate = 999999 }, GetSamplePath(11), 82000210);
        }

        [Test]
        public void Day12()
        {
            TestOneStar(new Day12HotSprings(), GetSamplePath(12), 21);
            TestOneStar(new Day12HotSprings(), GetSamplePath(12, "b"), 150);
            TestTwoStars(new Day12HotSprings(), GetSamplePath(12), 525152);
        }

        [Test]
        public void Day13()
        {
            TestOneStar(new Day13MirrorValley(), GetSamplePath(13), 405);
            TestTwoStars(new Day13MirrorValley(), GetSamplePath(13), 400);
        }

        [Test]
        public void Day14()
        {
            TestOneStar(new Day14RockyBalance(), GetSamplePath(14), 136);
            //TODO TestTwoStars(new Day14RockyBalance(), GetSamplePath(14), ??);
        }

        [Test]
        public void Day15()
        {
            TestOneStar(new Day15Manual(), GetSamplePath(15), 1320);
            TestOneStar(new Day15Manual(), GetSamplePath(15, "b"), 52);
            TestTwoStars(new Day15Manual(), GetSamplePath(15), 145);
        }

        [Test]
        public void Day16()
        {
            TestOneStar(new Day16EnergyBeam(), GetSamplePath(16), 46);
            TestTwoStars(new Day16EnergyBeam(), GetSamplePath(16), 51);
        }

        [Test]
        public void Day17()
        {
            TestOneStar(new Day17Crucible(), GetSamplePath(17), 102);
            TestTwoStars(new Day17Crucible(), GetSamplePath(17), 94);
        }

        [Test]
        public void Day18()
        {
            TestOneStar(new Day18LavaLagoon(), GetSamplePath(18), 62);
            TestTwoStars(new Day18LavaLagoon(), GetSamplePath(18), 952408144115);
        }

        [Test]
        public void Day19()
        {
            TestOneStar(new Day19ThingsSorter(), GetSamplePath(19), 19114);
            TestTwoStars(new Day19ThingsSorter(), GetSamplePath(19), 167409079868000);
        }

        [Test]
        public void Day20()
        {
            TestOneStar(new Day20Pulse(), GetSamplePath(20, "a"), 32000000);
            TestOneStar(new Day20Pulse(), GetSamplePath(20, "b"), 11687500);
            TestTwoStars(new Day20Pulse(), GetSamplePath(20, "b"), 1);
        }

        [Test]
        public void Day21()
        {
            TestOneStar(new Day21Garden { Steps = 6 }, GetSamplePath(21), 16);
            TestTwoStars(new Day21Garden { Steps = 6 }, GetSamplePath(21), 16);
            TestTwoStars(new Day21Garden { Steps = 10 }, GetSamplePath(21), 50);
            TestTwoStars(new Day21Garden { Steps = 50 }, GetSamplePath(21), 1594);
            TestTwoStars(new Day21Garden { Steps = 100 }, GetSamplePath(21), 6536);
            TestTwoStars(new Day21Garden { Steps = 500 }, GetSamplePath(21), 167004);
            TestTwoStars(new Day21Garden { Steps = 1000 }, GetSamplePath(21), 668697);
        }

        [Test]
        public void Day22()
        {
            TestOneStar(new Day22SandTetris(), GetSamplePath(22), 5);
            TestTwoStars(new Day22SandTetris(), GetSamplePath(22), 7);
        }
    }
}