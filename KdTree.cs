namespace csharp;

internal readonly record struct Point(double X, double Y);

internal sealed class KdTree<TPoint> where TPoint : struct
{
    public static void Example()
    {
        var points = new Point[]
        {
            new(2, 3),
            new(4, 7),
            new(5, 4),
            new(9, 6),
            new(8, 1),
            new(7, 2),
            new(3, 9),
            new(6, 8),
            new(1, 5),
            new(6, 2),
            new(3, 6),
            new(7, 8),
        };

        var queries = new (Point query, Point answer)[]
        {
            (new(1, 1), new(2, 3)),
            (new(6, 7), new(6, 8)),
            (new(7, 5), new(5, 4)),
            (new(3, 3), new(2, 3)),
            (new(8, 2), new(8, 1)),
            (new(5, 6), new(4, 7)),
            (new(4, 2), new(6, 2)),
            (new(9, 9), new(7, 8)),
            (new(3, 5), new(3, 6)),
            (new(6, 6), new(6, 8)),
            (new(2, 8), new(3, 9)),
            (new(7, 7), new(7, 8)),
        };


        var kdTree = new KdTree<Point>(points, []);

        foreach (var (query, answer) in queries)
        {
            var nearest = kdTree.FindNearest(query);
            if (nearest == answer) continue;
            Console.WriteLine($"query: {query}; answer: {answer}; got: {nearest?.ToString() ?? "null"}");
        }
    }

    private class Node
    {
        public required TPoint Point { get; init; }
        public required Node? Less { get; init; }
        public required Node? More { get; init; }
    }

    private readonly Node? _root;
    private readonly Func<TPoint, double>[] _extractors;

    private double KExtractor(TPoint point, int depth) => _extractors[depth % _extractors.Length].Invoke(point);

    public KdTree(IEnumerable<TPoint> points, IEnumerable<Func<TPoint, double>> extractors)
    {
        _extractors = extractors.ToArray();
        if (_extractors.Length == 0) throw new ArgumentException("no dimension component extractors given", nameof(extractors));
        _root = BuildTree(points, 0);
        return;

        Node? BuildTree(IEnumerable<TPoint> currentPoints, int depth)
        {
            var sortedPoints = currentPoints.OrderBy(p => KExtractor(p, depth)).ToArray();
            if (sortedPoints.Length == 0) return null;
            var medianIndex = sortedPoints.Length / 2;

            return new Node
            {
                Point = sortedPoints[medianIndex],
                Less = BuildTree(sortedPoints[..medianIndex], depth + 1),
                More = BuildTree(sortedPoints[(medianIndex + 1)..], depth + 1),
            };
        }
    }

    private TPoint? FindNearest(TPoint query)
    {
        return Find(_root, 0);

        double SquaredDistanceTo(TPoint point) => _extractors.Select(e => e(query) - e(point)).Sum(d => d * d);

        TPoint ClosestOf(TPoint p1, TPoint p2) => new[] { p1, p2 }.MinBy(SquaredDistanceTo);

        TPoint? Find(Node? node, int depth)
        {
            if (node is null) return null;

            var (queryK, nodeK) = (KExtractor(query, depth), KExtractor(node.Point, depth));
            var (nextBranch, otherBranch) = queryK < nodeK ? (node.Less, node.More) : (node.More, node.Less);

            var best = Find(nextBranch, depth + 1) is { } found ? ClosestOf(found, node.Point) : node.Point;
            if (queryK - nodeK is var distanceK && distanceK * distanceK >= SquaredDistanceTo(best)) return best;

            return Find(otherBranch, depth + 1) is { } alternative ? ClosestOf(alternative, best) : best;
        }
    }
}
