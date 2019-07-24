using System;
using System.Collections.Generic;
using System.Linq;

namespace DBSCAN
{
    public class Dbscan<TPointType, TMetricType> where TMetricType : IComparable
    {
        private const int Undefined = -2;
        private const int Noise = -1;

        private readonly Func<TPointType, TPointType, TMetricType> _distFunc;
        private readonly TMetricType _epsilon;
        private readonly int _minNeighbors;

        private class ClusterPoint
        {
            public readonly TPointType Point;
            public int Cluster;

            public ClusterPoint(TPointType point)
            {
                Point = point;
                Cluster = Undefined;
            }
        }

        public Dbscan(Func<TPointType, TPointType, TMetricType> distFunc, TMetricType epsilon, int minNeighbors)
        {
            _distFunc = distFunc;
            _epsilon = epsilon;
            _minNeighbors = minNeighbors;
        }

        public Dictionary<int, List<TPointType>> Cluster(IEnumerable<TPointType> points)
        {
            var clustered = Scan(points);
            return Split(clustered);
        }

        private IEnumerable<ClusterPoint> Scan(IEnumerable<TPointType> points)
        {
            var pts = points.Select(p => new ClusterPoint(p)).ToArray();
            var currentCluster = 0;

            for (var i = 0; i < pts.Length; ++i)
            {
                var currentPoint = pts.ElementAt(i);
                if (currentPoint.Cluster != Undefined)
                    continue;

                var neighbors = RangeQuery(pts, currentPoint);
                if (neighbors.Count < _minNeighbors)
                {
                    currentPoint.Cluster = Noise;
                    continue;
                }

                currentPoint.Cluster = currentCluster;
                neighbors.Remove(currentPoint);

                // Propagate to neighbors and neighbors' neighbors
                for (var j = 0; j < neighbors.Count; ++j)
                {
                    var qLabel = neighbors[j];
                    if (qLabel.Cluster == Noise)
                        qLabel.Cluster = currentCluster;
                    else if (qLabel.Cluster != Undefined)
                        continue;

                    qLabel.Cluster = currentCluster;
                    var qNeighbors = RangeQuery(pts, qLabel);
                    if (qNeighbors.Count >= _minNeighbors)
                    {
                        neighbors.AddRange(qNeighbors);
                    }
                }

                currentCluster += 1;
            }

            return pts;
        }

        private List<ClusterPoint> RangeQuery(IEnumerable<ClusterPoint> pts, ClusterPoint currentPoint)
        {
            return pts.Where(x => _distFunc(currentPoint.Point, x.Point).CompareTo(_epsilon) <= 0).ToList();
        }

        private static Dictionary<int, List<TPointType>> Split(IEnumerable<ClusterPoint> clustered)
        {
            var dic = new Dictionary<int, List<TPointType>>();

            foreach (var p in clustered)
            {
                var cluster = p.Cluster;
                if (!dic.ContainsKey(cluster))
                {
                    dic[cluster] = new List<TPointType>();
                }

                dic[cluster].Add(p.Point);
            }

            return dic;
        }
    }
}