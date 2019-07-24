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

            double EuclideanDistance(Point p1, Point p2) =>
                Math.Sqrt(Math.Pow(p1.Item1 - p2.Item1, 2) + Math.Pow(p1.Item2 - p2.Item2, 2));

            var clustered = new Dbscan<Point, double>(EuclideanDistance, epsilon, minNeighbors);
            var clusters = clustered.Cluster(points);

            foreach (var cluster in clusters)
            {
                Console.WriteLine($"Cluster no. {cluster.Key} has {cluster.Value.Count} points");
            }
        }

        private static List<Point> MakeGaussianCircle(double radius, int numPoint)
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