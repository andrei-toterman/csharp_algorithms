namespace csharp;

public static class ShortestDistance
{
    public static void Example()
    {
        var input = File.OpenText("input.txt");
        var t = int.Parse(input.ReadLine()!.Trim());
        while (t-- > 0)
        {
            var firstMultipleInput = input.ReadLine()!.TrimEnd().Split(' ');
            var n = int.Parse(firstMultipleInput[0]);
            var m = int.Parse(firstMultipleInput[1]);

            var edges = new List<List<int>>();
            while (m-- > 0) edges.Add(input.ReadLine()!.TrimEnd().Split(' ').ToList().Select(int.Parse).ToList());
            var s = int.Parse(input.ReadLine()!.Trim());

            try
            {
                Console.WriteLine(string.Join(" ", ShortestReach1(n, edges, s)));
                Console.WriteLine(string.Join(" ", ShortestReach2(n, edges, s)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public static List<int> ShortestReach1(int n, List<List<int>> inputEdges, int s)
    {
        var edges = new Dictionary<int, int>[n];
        for (var i = 0; i < n; i++) edges[i] = [];
        foreach (var edge in inputEdges)
        {
            var v1 = edge[0] - 1;
            var v2 = edge[1] - 1;
            var w = edge[2];
            edges[v1].TryAdd(v2, int.MaxValue);
            edges[v1][v2] = int.Min(w, edges[v1][v2]);
            edges[v2].TryAdd(v1, int.MaxValue);
            edges[v2][v1] = int.Min(w, edges[v2][v1]);
        }

        var distances = Enumerable.Repeat(int.MaxValue, n).ToList();
        distances[s - 1] = 0;
        var visited = new HashSet<int>();
        var queue = new PriorityQueue<int, int>([(s - 1, 0)]);

        while (queue.TryDequeue(out var from, out _))
        {
            if (!visited.Add(from)) continue;
            foreach (var (to, weight) in edges[from])
            {
                if (distances[to] <= distances[from] + weight) continue;
                distances[to] = distances[from] + weight;
                queue.Enqueue(to, distances[to]);
            }
        }

        distances = distances.Select(d => d == int.MaxValue ? -1 : d).ToList();
        return distances.Where((_, i) => i != s - 1).ToList();
    }

    static List<int> ShortestReach2(int n, List<List<int>> inputEdges, int s)
    {
        var edges = new int[n, n];
        foreach (var edge in inputEdges)
        {
            var v1 = edge[0] - 1;
            var v2 = edge[1] - 1;
            var w = edge[2];
            var ew1 = edges[v1, v2];
            if (ew1 == 0 || w < ew1) edges[v1, v2] = w;
            var ew2 = edges[v2, v1];
            if (ew2 == 0 || w < ew2) edges[v2, v1] = w;
        }

        var distances = Enumerable.Repeat(int.MaxValue, n).ToList();
        distances[s - 1] = 0;
        var visited = new HashSet<int>();
        var queue = new PriorityQueue<int, int>([(s - 1, 0)]);

        while (queue.TryDequeue(out var from, out _))
        {
            if (!visited.Add(from)) continue;
            foreach (var (to, weight) in EdgeValues(edges, from))
            {
                if (distances[to] <= distances[from] + weight) continue;
                distances[to] = distances[from] + weight;
                queue.Enqueue(to, distances[to]);
            }
        }

        distances = distances.Select(d => d == int.MaxValue ? -1 : d).ToList();
        return distances.Where((_, i) => i != s - 1).ToList();
    }

    static IEnumerable<(int, int)> EdgeValues(int[,] edges, int from)
    {
        for (var i = 0; i < edges.GetLength(0); i++)
        {
            var w = edges[i, from];
            if (w > 0) yield return (i, w);
        }
    }
}
