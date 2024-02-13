using System.Text;
using System.Text.RegularExpressions;
using Range = AoCTools.Numbers.Range;

namespace AoC2023.Structures
{
    public class ThingWorkflow
    {
        public ThingWorkflow(string name, string[] conditions)
        {
            Name = name;
            Conditions = conditions.Select(ThingCondition.Build).ToArray();
        }

        public string Name { get; }
        public ThingCondition[] Conditions { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($">>> Workflow {Name}:");
            foreach (var condition in Conditions)
                sb.AppendLine(condition.ToString());
            return sb.ToString();
        }

        public string TryThing(Thing thing)
        {
            foreach (var condition in Conditions)
                if (condition.TryThing(thing, out var consequence))
                    return consequence;
            throw new Exception($"Workflow didn't end!");
        }
    }

    public abstract class ThingCondition
    {
        private static readonly Regex ComparisonRegex = new Regex(@"(?<prop>[a-z])(?<comp><|>)(?<val>[0-9]+):(?<csq>[a-zA-Z]+)", RegexOptions.Compiled);
        public static ThingCondition Build(string conditionStr)
        {
            ThingCondition newCondition = null;

            var match = ComparisonRegex.Match(conditionStr);
            if (match.Success)
            {
                newCondition = new ThingComparisonCondition(
                    match.Groups["prop"].Value[0],
                    match.Groups["comp"].Value[0],
                    int.Parse(match.Groups["val"].Value),
                    match.Groups["csq"].Value);
            }
            else
            {
                newCondition = new ThingDirectCondition(conditionStr);
            }

            newCondition.RawCondition = conditionStr;
            return newCondition;
        }

        public string RawCondition { get; private set; }
        public string RawConsequence { get; protected set; }

        public virtual bool TryThing(Thing thing, out string consequence)
        {
            throw new NotImplementedException();
        }
    }

    public class ThingComparisonCondition : ThingCondition
    {
        public ThingComparisonCondition(char property, char comparator, int value, string consequence)
        {
            RawProperty = property;
            RawComparator = comparator;
            Value = value;
            RawConsequence = consequence;
        }

        public char RawProperty { get; }
        public char RawComparator { get; }
        public int Value { get; }

        public override string ToString()
        {
            return $"{RawProperty} {RawComparator} {Value} => {RawConsequence}";
        }

        public override bool TryThing(Thing thing, out string consequence)
        {
            var property = GetNeededThingPropertyValue(thing);
            consequence = RawConsequence;
            switch (RawComparator)
            {
                case '>': return property > Value;
                case '<': return property < Value;
            }
            throw new Exception($"Unknown comparator '{RawComparator}'");
        }

        public Range GetValidRange(int min = 0, int max = 4000)
        {
            switch (RawComparator)
            {
                case '>': return Range.CreateFromMinMax(Value + 1, max);
                case '<': return Range.CreateFromMinMax(min, Value - 1);
            }
            throw new Exception($"Unknown comparator '{RawComparator}'");
        }

        private int GetNeededThingPropertyValue(Thing thing)
        {
            switch (RawProperty)
            {
                case 'x': return thing.X;
                case 'm': return thing.M;
                case 'a': return thing.A;
                case 's': return thing.S;
            }
            throw new Exception($"Unknown property {RawProperty}");
        }
    }

    public class ThingDirectCondition : ThingCondition
    {
        public ThingDirectCondition(string consequence)
        {
            RawConsequence = consequence;
        }

        public override string ToString()
        {
            return $"Direct consequence => {RawConsequence}";
        }

        public override bool TryThing(Thing thing, out string consequence)
        {
            consequence = RawConsequence;
            return true;
        }
    }

    public class Thing
    {
        public Thing(int x, int m, int a, int s)
        {
            X = x;
            M = m;
            A = a;
            S = s;
        }

        public int X { get; }
        public int M { get; }
        public int A { get; }
        public int S { get; }
        public long Rating => X + M + A + S;

        public override string ToString()
        {
            return $"X={X} M={M} A={A} S={S}";
        }
    }

    public class ThingRange
    {
        public ThingRange() { }

        public ThingRange(ThingRange copy)
        {
            X = Range.CreateFromCopy(copy.X);
            M = Range.CreateFromCopy(copy.M);
            A = Range.CreateFromCopy(copy.A);
            S = Range.CreateFromCopy(copy.S);
        }

        public Range X { get; set; }
        public Range M { get; set; }
        public Range A { get; set; }
        public Range S { get; set; }

        public override string ToString()
        {
            return $"X = {X} || M = {M} || A = {A} || S = {S}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ThingRange other))
                return false;

            return X.Equals(other.X) && M.Equals(other.M) && A.Equals(other.A) && S.Equals(other.S);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() * M.GetHashCode() * A.GetHashCode() * S.GetHashCode();
        }
    }
}