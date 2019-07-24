using System;
using System.Collections.Generic;
using DBSCAN;

namespace App
{
    using Point = Tuple<double, double>;

    internal static class Program
    {
        public static void Main()
        {
            const double epsilon = 0.5;
            const int minNeighbors = 2;
            var points = new List<Point>();

            points.AddRange(MakeGaussianCircle(1, 100));
            points.AddRange(MakeGaussianCircle(2, 100));
            points.AddRange(MakeGaussianCircle(3, 100));

            double EuclideanDistance(Point p1, Point p2)
            {
                var (x1, y1) = p1;
                var (x2, y2) = p2;
                return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            }

            var clustered = new Dbscan<Point, double>(EuclideanDistance, epsilon, minNeighbors);
            var clusters = clustered.Cluster(points);

            foreach (var cluster in clusters)
                Console.WriteLine($"Cluster no. {cluster.Key} has {cluster.Value.Count} points");
        }

        private static IEnumerable<Point> MakeGaussianCircle(double radius, int numPoint)
        {
            var inc = 2 * Math.PI / numPoint;
            var numStep = Math.Ceiling(2 * Math.PI / inc);
            var pts = new List<Point>();
            for (var i = 0; i < numStep; i++)
            {
                var x = i * inc;
                var pt = new Point(radius * Math.Cos(x), radius * Math.Sin(x));
                pts.Add(pt);
            }

            return pts;
        }
    }
}