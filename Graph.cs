namespace csharp;

public sealed class Graph<TVertex> where TVertex : notnull
{
    public static void Example()
    {
        var graph = new Graph<char>(['a', 'b', 'c', 'd', 'e', 'f']);
        graph.AddEdge(from: 'a', to: 'b', weight: 4);
        graph.AddEdge(from: 'a', to: 'c', weight: 5);
        graph.AddEdge(from: 'b', to: 'c', weight: 11);
        graph.AddEdge(from: 'b', to: 'd', weight: 9);
        graph.AddEdge(from: 'b', to: 'e', weight: 7);
        graph.AddEdge(from: 'c', to: 'e', weight: 3);
        graph.AddEdge(from: 'd', to: 'e', weight: 13);
        graph.AddEdge(from: 'd', to: 'f', weight: 2);
        graph.AddEdge(from: 'e', to: 'f', weight: 6);

        Console.WriteLine("Vertex\tDistance\tPath");
        foreach (var (vertex, distance, path) in graph.Dijkstra(startingNode: 'a'))
        {
            Console.WriteLine($"{vertex}\t{distance}\t\t{string.Join(" -> ", path)}");
        }
    }

    private readonly Dictionary<int, int>[] _connections;
    private readonly Dictionary<TVertex, int> _mappings;
    private readonly TVertex[] _vertices;

    public Graph(IEnumerable<TVertex> vertices)
    {
        _vertices = vertices.Distinct().ToArray();
        _mappings = _vertices.Select((v, i) => (v, i)).ToDictionary();
        _connections = new Dictionary<int, int>[_vertices.Length];
        for (var i = 0; i < _vertices.Length; i++) _connections[i] = [];
    }

    public void AddEdge(TVertex from, TVertex to, int weight = 0)
    {
        _connections[_mappings[from]].Add(_mappings[to], weight);
        _connections[_mappings[to]].Add(_mappings[from], weight);
    }

    public IEnumerable<TVertex[]> ConnectedComponents()
    {
        var notVisited = _vertices.Select((_, i) => i).ToHashSet();
        while (notVisited.Count > 0)
        {
            var connected = Bfs(notVisited.First()).ToArray();
            yield return connected.Select(i => _vertices[i]).ToArray();
            notVisited.SymmetricExceptWith(connected);
        }
    }

    private IEnumerable<int> Bfs(int start)
    {
        var visited = new bool[_vertices.Length];
        var queue = new Queue<int>([start]);
        while (queue.TryDequeue(out var node))
        {
            if (visited[node]) continue;
            visited[node] = true;
            yield return node;
            foreach (var neighbor in _connections[node].Keys) queue.Enqueue(neighbor);
        }
    }

    private IEnumerable<int> Dfs(int start)
    {
        var visited = new bool[_vertices.Length];
        var queue = new Stack<int>([start]);
        while (queue.TryPop(out var node))
        {
            if (visited[node]) continue;
            visited[node] = true;
            yield return node;
            foreach (var neighbor in _connections[node].Keys) queue.Push(neighbor);
        }
    }


    public IEnumerable<(TVertex vertex, int distance, TVertex[] path)> Dijkstra(TVertex startingNode)
    {
        var startingNodeId = _mappings[startingNode];
        var distance = _vertices.Select((_, i) => i == startingNodeId ? 0 : int.MaxValue).ToArray();
        var path = _vertices.Select(_ => -1).ToArray();
        var visited = _vertices.Select(_ => false).ToArray();
        var queue = new PriorityQueue<int, int>([(startingNodeId, 0)]);

        while (queue.TryDequeue(out var from, out _))
        {
            if (visited[from]) continue;
            visited[from] = true;
            foreach (var (to, weight) in _connections[from])
            {
                if (distance[to] <= distance[from] + weight) continue;
                distance[to] = distance[from] + weight;
                path[to] = from;
                queue.Enqueue(to, distance[to]);
            }
        }

        return distance.Select((d, i) => (vertex: _vertices[i], distance: d, path: PathTo(i).ToArray()));

        IEnumerable<TVertex> PathTo(int current)
        {
            var stack = new Stack<TVertex>();
            while (current != -1)
            {
                stack.Push(_vertices[current]);
                current = path[current];
            }

            return stack;
        }
    }
}
