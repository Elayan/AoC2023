using System.Collections.Generic;
using System.Linq;
using AoC2023.Structures;
using AoCTools.File;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2023.Workers
{
    public class Day09OasisStability : WorkerBase
    {
        private OasisReport[] _reports;
        public override object Data => _reports;

        protected override void ProcessDataLines()
        {
            var reports = new List<OasisReport>();
            foreach (var line in DataLines)
            {
                reports.Add(new OasisReport
                {
                    Values = line.Split(' ').Select(long.Parse).ToArray()
                });
            }

            _reports = reports.ToArray();
        }

        protected override long WorkOneStar_Implementation()
        {
            long sum = 0;
            foreach (var report in _reports)
            {
                var reportAndSimplifications = new List<OasisReport>() { report };
                reportAndSimplifications.AddRange(GenerateSimplifications(report));

                Logger.Log("Predicting forward:");
                long extraValue = 0;
                for (int i = reportAndSimplifications.Count - 1; i > 0; i--)
                {
                    Logger.Log($"{reportAndSimplifications[i]} [{extraValue}]");
                    extraValue += reportAndSimplifications[i - 1].Values.Last();
                }
                Logger.Log($"{reportAndSimplifications[0]} [{extraValue}]");
                sum += extraValue;
            }

            Logger.Log($"Total forward prediction = {sum}", SeverityLevel.Always);
            return sum;
        }

        protected override long WorkTwoStars_Implementation()
        {
            long sum = 0;
            foreach (var report in _reports)
            {
                var reportAndSimplifications = new List<OasisReport>() { report };
                reportAndSimplifications.AddRange(GenerateSimplifications(report));

                Logger.Log("Predicting backwards:");
                long extraValue = 0;
                for (int i = reportAndSimplifications.Count - 1; i > 0; i--)
                {
                    Logger.Log($"[{extraValue}] {reportAndSimplifications[i]}");
                    extraValue = reportAndSimplifications[i - 1].Values.First() - extraValue;
                }
                Logger.Log($"[{extraValue}] {reportAndSimplifications[0]}");
                sum += extraValue;
            }

            Logger.Log($"Total backward prediction = {sum}", SeverityLevel.Always);
            return sum;
        }

        private static OasisReport[] GenerateSimplifications(OasisReport report)
        {
            Logger.Log("Simplifying:");
            Logger.Log(report.ToString());

            var simplifications = new List<OasisReport>();
            var curReport = report;
            while (curReport.IsSimplifiable())
            {
                var values = new List<long>();
                for (int i = 1; i < curReport.Values.Length; i++)
                    values.Add(curReport.Values[i] - curReport.Values[i-1]);
                simplifications.Add(new OasisReport
                {
                    Values = values.Any()
                        ? values.ToArray()
                        : new long[] { 0 }
                });
                curReport = simplifications.Last();
                Logger.Log(curReport.ToString());
            }

            return simplifications.ToArray();
        }
    }
}