namespace csharp;

public sealed class AhoCorasick
{
    public static void Example()
    {
        var ac = new AhoCorasick(["a", "an", "ant", "anteater", "neat", "tea", "eat", "ate", "eater", "at"]);
        const string description = "anteater";
        foreach (var c in description) Console.Write($"  {c}");
        Console.WriteLine();
        foreach (var i in Enumerable.Range(0, description.Length)) Console.Write($" {i:00}");
        Console.WriteLine();

        foreach (var (word, index) in ac.Search(description)) Console.WriteLine($"{index:00}: {word}");
    }

    private class TrieNode
    {
        public readonly Dictionary<char, TrieNode> Children = [];
        public string? Word;
        public TrieNode? FailureLink;
    }

    private readonly TrieNode _root = new();

    public AhoCorasick(IEnumerable<string> patterns)
    {
        foreach (var pattern in patterns.Distinct())
        {
            var node = _root;
            foreach (var c in pattern)
            {
                node.Children.TryAdd(c, new TrieNode());
                node = node.Children[c];
            }

            node.Word = pattern;
        }

        var queue = new Queue<TrieNode>([_root]);
        while (queue.TryDequeue(out var node))
        {
            foreach (var (c, child) in node.Children)
            {
                queue.Enqueue(child);
                var failure = node.FailureLink;
                while (failure is not null && !failure.Children.ContainsKey(c)) failure = failure.FailureLink;
                child.FailureLink = failure?.Children.GetValueOrDefault(c, _root);
            }
        }
    }

    public IEnumerable<(string, int)> Search(string text)
    {
        var node = _root;
        foreach (var (c, i) in text.Select((c, i) => (c, i)))
        {
            do
            {
                if (node.Children.TryGetValue(c, out var child))
                {
                    node = child;
                    break;
                }

                node = node.FailureLink ?? _root;
            } while (node != _root);

            if (node.Word is { } word) yield return (word, i - word.Length + 1);
        }
    }
}
