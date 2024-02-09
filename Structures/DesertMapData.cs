using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC2023.Structures
{
    public enum DesertMapDirection
    {
        Left,
        Right
    }

    public class DesertMapData
    {
        private List<DesertMapDirection> _directions = new List<DesertMapDirection>();
        public DesertMapDirection[] Directions => _directions.ToArray();

        public void AddDirection(char d)
        {
            _directions.Add(d == 'R' ? DesertMapDirection.Right : DesertMapDirection.Left);
        }

        public DesertMapDirection GetDirection(long index)
        {
            long realIndex = index % _directions.Count;
            return _directions[(int)realIndex];
        }

        private List<DesertMapPoint> _allPoints = new List<DesertMapPoint>();
        public DesertMapPoint StartingPoint => _allPoints.First(p => p.Name == "AAA");
        public DesertMapPoint[] GhostStartingPoints => _allPoints.Where(p => p.Name.EndsWith("A")).ToArray();

        public void AddPoint(string name, string leftPointName, string rightPointName)
        {
            var point = FindOrCreatePoint(name);

            point.LeftPoint = FindOrCreatePoint(leftPointName);
            point.RightPoint = FindOrCreatePoint(rightPointName);
        }

        private DesertMapPoint FindOrCreatePoint(string name)
        {
            var point = _allPoints.FirstOrDefault(p => p.Name == name);
            if (point != null)
                return point;

            point = new DesertMapPoint { Name = name };
            _allPoints.Add(point);
            return point;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("===== DESERT MAP =====");
            sb.AppendLine("== Directions ==");
            sb.AppendLine(string.Join(", ", _directions));
            sb.AppendLine("== Points ==");
            foreach (var point in _allPoints)
                sb.AppendLine(point.ToString());
            return sb.ToString();
        }
    }

    public class DesertMapPoint
    {
        public string Name { get; set; }
        public DesertMapPoint LeftPoint { get; set; }
        public DesertMapPoint RightPoint { get; set; }

        public override string ToString()
        {
            return $"{Name} => {LeftPoint.Name} or {RightPoint.Name}";
        }
    }

    public class SimplifiedGhostPath
    {
        private SimplifiedGhostPath() { }
        public SimplifiedGhostPath(long[] arrivals)
        {
            AllFirstArrivals = arrivals;
            _offset = arrivals[0];
            _arrivalValues = arrivals.Select(a => a - _offset).ToArray();
        }

        public long _offset { get; set; }
        public long[] _arrivalValues { get; set; }
        public long[] AllFirstArrivals { get; set; }

        // iterator
        private int _index = 0;
        private long _repeater = 0;

        public void ResetIterator()
        {
            _index = 0;
            _repeater = 1;
        }

        public long GetNextArrival()
        {
            var val = _arrivalValues[_index] + _repeater * _arrivalValues.Last() + _offset;
            _index++;
            if (_index == _arrivalValues.Length)
            {
                _index = 1;
                _repeater++;
            }
            return val;
        }
    }
}