namespace csharp;

public sealed class Trie
{
    public static void Example()
    {
        var input = File.OpenText("input.txt");
        var queriesRows = int.Parse(input.ReadLine()!.Trim());
        var queries = new List<List<string>>();
        while (queriesRows-- > 0) queries.Add(input.ReadLine()!.TrimEnd().Split(' ').ToList());

        var trie = new Trie();
        var results = new List<int>();
        foreach (var query in queries)
        {
            if (query is not [var action, var text]) continue;
            if (action == "add") trie.Add(text);
            if (action == "find") results.Add(trie.Find(text));
        }

        Console.WriteLine(string.Join('\n', results));
    }

    public class Node
    {
        public bool IsWord;
        public readonly Dictionary<char, Node> Children = [];
    }

    private readonly Node _root = new();

    public void Add(string name)
    {
        var currentNode = _root;
        foreach (var letter in name)
        {
            currentNode.Children.TryAdd(letter, new Node());
            currentNode = currentNode.Children[letter];
        }

        currentNode.IsWord = true;
    }

    public int Find(string prefix)
    {
        var nodeForPrefix = _root;
        foreach (var c in prefix)
        {
            if (!nodeForPrefix.Children.TryGetValue(c, out var child)) return 0;
            nodeForPrefix = child;
        }

        var queue = new Queue<Node>([nodeForPrefix]);
        var count = 0;
        while (queue.TryDequeue(out var node))
        {
            if (node.IsWord) count++;
            foreach (var child in node.Children.Values) queue.Enqueue(child);
        }

        return count;
    }
}
