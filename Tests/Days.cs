using AoC2023.Workers;
using AoCTools.Loggers;
using AoCTools.Workers;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace AoC2023_Exec
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class Days
    {
        private string GetDataPath(int day, string append = "")
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Days", $"day{day:00}{append}");
        }

        private void TestOneStar(IWorker worker, string dataPath, long expectedResult)
        {
            var work = worker.WorkOneStar(dataPath, SeverityLevel.Always);
            Assert.That(work, Is.EqualTo(expectedResult), $"One Star returned {work}, expected {expectedResult}.");
        }

        private void TestTwoStars(IWorker worker, string dataPath, long expectedResult)
        {
            var work = worker.WorkTwoStars(dataPath, SeverityLevel.Always);
            Assert.That(work, Is.EqualTo(expectedResult), $"Two Stars returned {work}, expected {expectedResult}.");
        }

        [Test]
        public void Day01()
        {
            TestOneStar(new Day01Calibrator(), GetDataPath(1), 54877);
            TestTwoStars(new Day01Calibrator(), GetDataPath(1), 54100);
        }

        [Test]
        public void Day02()
        {
            TestOneStar(new Day02CubeGame(), GetDataPath(2), 2162);
            TestTwoStars(new Day02CubeGame(), GetDataPath(2), 72513);
        }

        [Test]
        public void Day03()
        {
            TestOneStar(new Day03Gondola(), GetDataPath(3), 520135);
            TestTwoStars(new Day03Gondola(), GetDataPath(3), 72514855);
        }

        [Test]
        public void Day04()
        {
            TestOneStar(new Day04Scratchcard(), GetDataPath(4), 21959);
            TestTwoStars(new Day04Scratchcard(), GetDataPath(4), 5132675);
        }

        [Test]
        public void Day05()
        {
            TestOneStar(new Day05Almanac(), GetDataPath(5), 484023871);
            TestTwoStars(new Day05Almanac { ReadAsSeedRanges = true }, GetDataPath(5), 46294175);
        }

        [Test]
        public void Day06()
        {
            TestOneStar(new Day06BoatRace(), GetDataPath(6), 449820);
            TestTwoStars(new Day06BoatRace { ReadWithoutSpaces = true }, GetDataPath(6), 42250895);
        }

        [Test]
        public void Day07()
        {
            TestOneStar(new Day07CamelCards(), GetDataPath(7), 253313241);
            TestTwoStars(new Day07CamelCards { UseJokers = true }, GetDataPath(7), 253362743);
        }

        [Test]
        public void Day08()
        {
            TestOneStar(new Day08DesertMap(), GetDataPath(8), 12083);
            TestTwoStars(new Day08DesertMap(), GetDataPath(8), 13385272668829);
        }

        [Test]
        public void Day09()
        {
            TestOneStar(new Day09OasisStability(), GetDataPath(9), 1921197370);
            TestTwoStars(new Day09OasisStability(), GetDataPath(9), 1124);
        }

        [Test]
        public void Day10()
        {
            TestOneStar(new Day10MetalPipes(), GetDataPath(10), 6831);
            TestTwoStars(new Day10MetalPipes(), GetDataPath(10), 305);
        }

        [Test]
        public void Day11()
        {
            TestOneStar(new Day11Galaxy(), GetDataPath(11), 9418609);
            TestTwoStars(new Day11Galaxy { ExpansionRate = 999999 }, GetDataPath(11), 593821230983);
        }

        [Test]
        public void Day12()
        {
            TestOneStar(new Day12HotSprings(), GetDataPath(12), 7857);
            //TODO TestTwoStars(new Day12HotSprings(), GetDataPath(11), ???);
        }

        [Test]
        public void Day13()
        {
            TestOneStar(new Day13MirrorValley(), GetDataPath(13), 33122);
            TestTwoStars(new Day13MirrorValley(), GetDataPath(13), 32312);
        }

        [Test]
        public void Day14()
        {
            TestOneStar(new Day14RockyBalance(), GetDataPath(14), 106517);
            //TODO TestTwoStars(new Day14RockyBalance(), GetDataPath(14), ???);
        }

        [Test]
        public void Day15()
        {
            TestOneStar(new Day15Manual(), GetDataPath(15), 511498);
            TestTwoStars(new Day15Manual(), GetDataPath(15), 284674);
        }

        [Test]
        public void Day16()
        {
            TestOneStar(new Day16EnergyBeam(), GetDataPath(16), 7623);
            TestTwoStars(new Day16EnergyBeam(), GetDataPath(16), 8244);
        }

        [Test]
        public void Day17()
        {
            TestOneStar(new Day17Crucible(), GetDataPath(17), 1004);
            TestTwoStars(new Day17Crucible(), GetDataPath(17), 1171);
        }

        [Test]
        public void Day18()
        {
            TestOneStar(new Day18LavaLagoon(), GetDataPath(18), 68115);
            TestTwoStars(new Day18LavaLagoon(), GetDataPath(18), 71262565063800);
        }

        [Test]
        public void Day19()
        {
            TestOneStar(new Day19ThingsSorter(), GetDataPath(19), 397134);
            TestTwoStars(new Day19ThingsSorter(), GetDataPath(19), 127517902575337);
        }

        [Test]
        public void Day20()
        {
            TestOneStar(new Day20Pulse(), GetDataPath(20), 929810733);
            //TODO TestTwoStars(new Day20Pulse(), GetDataPath(20), ??);
        }

        [Test]
        public void Day21()
        {
            TestOneStar(new Day21Garden { Steps = 64 }, GetDataPath(21), 3746);
            //TODO TestTwoStars(new Day21Garden(), GetDataPath(21), ??);
        }

        [Test]
        public void Day22()
        {
            TestOneStar(new Day22SandTetris(), GetDataPath(22), 530);
            TestTwoStars(new Day22SandTetris(), GetDataPath(22), 93292);
        }
    }
}