using System.Text;
using System.Text.RegularExpressions;
using AoC2023.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;
using Range = AoCTools.Numbers.Range;

namespace AoC2023.Workers
{
    public class Day19ThingsSorter : WorkerBase
    {
        private static readonly Regex WorkflowRegex = new Regex(@"(?<name>[a-z]+){(?<cond>[^}]+)}", RegexOptions.Compiled);
        private static readonly Regex ThingRegex = new Regex(@"x=(?<x>[0-9]+),m=(?<m>[0-9]+),a=(?<a>[0-9]+),s=(?<s>[0-9]+)", RegexOptions.Compiled);

        private ThingWorkflow[] _workflows;
        private Thing[] _things;

        protected override void ProcessDataLines()
        {
            var workflowLines = DataLines.TakeWhile(l => !string.IsNullOrEmpty(l)).ToList();
            _workflows = workflowLines.Select(wl =>
            {
                var match = WorkflowRegex.Match(wl);
                return new ThingWorkflow(
                    match.Groups["name"].Value,
                    match.Groups["cond"].Value.Split(','));
            }).ToArray();

            var thingLines = DataLines.Skip(workflowLines.Count + 1);
            _things = thingLines.Select(tl =>
            {
                var match = ThingRegex.Match(tl);
                return new Thing(
                    int.Parse(match.Groups["x"].Value),
                    int.Parse(match.Groups["m"].Value),
                    int.Parse(match.Groups["a"].Value),
                    int.Parse(match.Groups["s"].Value));
            }).ToArray();
        }

        protected override long WorkOneStar_Implementation()
        {
            var acceptedThings = DoesItTasteGood();
            var rating = acceptedThings.Sum(at => at.Rating);
            Logger.Log($"Accepted total rating = {rating}", SeverityLevel.Always);
            return rating;
        }

        private Thing[] DoesItTasteGood()
        {
            var accepted = new List<Thing>();
            foreach (var thing in _things)
            {
                var nextWorkflowName = "in";
                while (nextWorkflowName != "A" && nextWorkflowName != "R")
                {
                    var workflow = _workflows.First(w => w.Name == nextWorkflowName);
                    nextWorkflowName = workflow.TryThing(thing);
                }

                if (nextWorkflowName != "A")
                    continue;

                accepted.Add(thing);
            }

            return accepted.ToArray();
        }

        protected override long WorkTwoStars_Implementation()
        {
            var result = WhatTasteGood(_workflows);
            Logger.Log($"Possible combinations = {result}", SeverityLevel.Always);
            return result;
        }

        private static long WhatTasteGood(ThingWorkflow[] workflows)
        {
            var tastingRanges = new List<Tuple<string, ThingRange>>
            {
                new Tuple<string, ThingRange>(
                    "in",
                    new ThingRange
                    {
                        X = Range.CreateFromMinMax(1L, 4000L),
                        M = Range.CreateFromMinMax(1L, 4000L),
                        A = Range.CreateFromMinMax(1L, 4000L),
                        S = Range.CreateFromMinMax(1L, 4000L)
                    }
                )
            };
            var acceptedRanges = new List<ThingRange>();

            while (tastingRanges.Any())
            {
                var tastingPair = tastingRanges.First();
                tastingRanges.Remove(tastingPair);

                if (tastingPair.Item1 == "R")
                {
                    Logger.Log("Rejection path.");
                    continue; // rejected
                }

                if (tastingPair.Item1 == "A")
                {
                    Logger.Log("Acceptance path.");
                    acceptedRanges.Add(tastingPair.Item2);
                    continue; // accepted
                }

                var tastingRange = tastingPair.Item2;
                Logger.Log($"Running workflow {tastingPair.Item1} for thing ranges {tastingRange}");

                var workflow = workflows.First(w => w.Name == tastingPair.Item1);
                foreach (var condition in workflow.Conditions)
                {
                    Logger.Log($"=> condition {condition}");
                    if (!(condition is ThingComparisonCondition comparison))
                    {
                        Logger.Log($"===> immediate validity to {condition.RawConsequence}");
                        tastingRanges.Add(new Tuple<string, ThingRange>(condition.RawConsequence, tastingRange));
                        break;
                    }

                    var validConditionRange = comparison.GetValidRange();
                    var validTastingThingRange = new ThingRange();

                    var extraThingRange = new ThingRange(tastingRange);
                    var usedExtraThingRange = false;

                    if (comparison.RawProperty == 'x')
                    {
                        ComputeUpdateRanges(tastingRange.X, validConditionRange,
                            out var validRange, out var leftRanges);
                        validTastingThingRange.X = validRange;
                        Logger.Log($"===> valid X range {validRange}");

                        if (leftRanges.Length == 0)
                        {
                            tastingRange.X = null;
                            Logger.Log($"===> left X range is empty");
                        }
                        else
                        {
                            tastingRange.X = leftRanges[0];
                            Logger.Log($"===> left X range {tastingRange.X}");
                        }

                        if (leftRanges.Length > 1)
                        {
                            extraThingRange.X = leftRanges[1];
                            usedExtraThingRange = true;
                            Logger.Log($"===> extra left X range {extraThingRange.X}");
                        }
                    }
                    else validTastingThingRange.X = Range.CreateFromCopy(tastingRange.X);

                    if (comparison.RawProperty == 'm')
                    {
                        ComputeUpdateRanges(tastingRange.M, validConditionRange,
                            out var validRange, out var leftRanges);
                        validTastingThingRange.M = validRange;
                        Logger.Log($"===> valid M range {validRange}");

                        if (leftRanges.Length == 0)
                        {
                            tastingRange.M = null;
                            Logger.Log($"===> left M range is empty");
                        }
                        else
                        {
                            tastingRange.M = leftRanges[0];
                            Logger.Log($"===> left M range {tastingRange.M}");
                        }

                        if (leftRanges.Length > 1)
                        {
                            extraThingRange.M = leftRanges[1];
                            usedExtraThingRange = true;
                            Logger.Log($"===> extra left M range {extraThingRange.M}");
                        }
                    }
                    else validTastingThingRange.M = Range.CreateFromCopy(tastingRange.M);

                    if (comparison.RawProperty == 'a')
                    {
                        ComputeUpdateRanges(tastingRange.A, validConditionRange,
                            out var validRange, out var leftRanges);
                        validTastingThingRange.A = validRange;
                        Logger.Log($"===> valid A range {validRange}");

                        if (leftRanges.Length == 0)
                        {
                            tastingRange.A = null;
                            Logger.Log($"===> left A range is empty");
                        }
                        else
                        {
                            tastingRange.A = leftRanges[0];
                            Logger.Log($"===> left A range {tastingRange.A}");
                        }

                        if (leftRanges.Length > 1)
                        {
                            extraThingRange.A = leftRanges[1];
                            usedExtraThingRange = true;
                            Logger.Log($"===> extra left A range {extraThingRange.A}");
                        }
                    }
                    else validTastingThingRange.A = Range.CreateFromCopy(tastingRange.A);

                    if (comparison.RawProperty == 's')
                    {
                        ComputeUpdateRanges(tastingRange.S, validConditionRange,
                            out var validRange, out var leftRanges);
                        validTastingThingRange.S = validRange;
                        Logger.Log($"===> valid S range {validRange}");

                        if (leftRanges.Length == 0)
                        {
                            tastingRange.S = null;
                            Logger.Log($"===> left S range is empty");
                        }
                        else
                        {
                            tastingRange.S = leftRanges[0];
                            Logger.Log($"===> left S range {tastingRange.S}");
                        }

                        if (leftRanges.Length > 1)
                        {
                            extraThingRange.S = leftRanges[1];
                            usedExtraThingRange = true;
                            Logger.Log($"===> extra left S range {extraThingRange.S}");
                        }
                    }
                    else validTastingThingRange.S = Range.CreateFromCopy(tastingRange.S);

                    // add valid range to further process
                    tastingRanges.Add(new Tuple<string, ThingRange>(comparison.RawConsequence, validTastingThingRange));

                    // we passed all possible values for one param, stop processing conditions
                    if (tastingRange.X == null || tastingRange.M == null || tastingRange.A == null || tastingRange.S == null)
                    {
                        Logger.Log("===> left range contains empty, discarding.");
                        break;
                    }

                    // add leftover to process again with same workflow while keeping processing conditions on the other half
                    if (usedExtraThingRange)
                    {
                        tastingRanges.Add(new Tuple<string, ThingRange>(tastingPair.Item1, extraThingRange));
                        Logger.Log("===> extra left range sent to reprocess.");
                    }
                }
            }

            if (Logger.ShowAboveSeverity == SeverityLevel.Never)
            {
                var sb = new StringBuilder();
                sb.AppendLine("=== Accepted ranges ===");
                foreach (var acceptedRange in acceptedRanges)
                    sb.AppendLine(acceptedRange.ToString());
                Logger.Log(sb.ToString());
            }

            var total = 0L;
            foreach (var acceptedRange in acceptedRanges)
                total += (long)(acceptedRange.X.Size * acceptedRange.M.Size * acceptedRange.A.Size * acceptedRange.S.Size);
            return total;
        }

        private static void ComputeUpdateRanges(Range baseRange, Range validConditionRange,
            out Range validRange, out Range[] leftRanges)
        {
            validRange = Range.CreateFromIntersecting(baseRange, validConditionRange);
            leftRanges = Range.CreateFromExcluding(baseRange, validConditionRange);
        }
    }
}