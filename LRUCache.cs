namespace csharp;

public class LruCache<TK, TV>(int capacity)
    where TK : notnull
{
    private class NodeData
    {
        public required TK Key { get; init; }
        public required TV Value { get; set; }
        public required bool Popular { get; set; } = false;
    }


    private readonly Dictionary<TK, LinkedListNode<NodeData>> _cache = [];
    private readonly LinkedList<NodeData> _usageOrder = [];

    public TV? Get(TK key)
    {
        if (!_cache.TryGetValue(key, out var value)) return default;
        _usageOrder.Remove(value);
        _usageOrder.AddFirst(value);
        return value.Value.Value;
    }

    public void Put(TK key, TV value)
    {
        if (_cache.TryGetValue(key, out var node))
        {
            _usageOrder.Remove(node);
            _usageOrder.AddFirst(node);
            node.Value.Popular = true;
            return;
        }

        if (_cache.Count >= capacity && _usageOrder.Last is { } lruNode)
        {
            _usageOrder.RemoveLast();
            _cache.Remove(lruNode.Value.Key);
        }

        // Add new item to the cache and the usage order
        var newNode = new LinkedListNode<NodeData>(new NodeData { Key = key, Value = value, Popular = false });
        _usageOrder.AddFirst(newNode);
        _cache[key] = newNode;
    }
}

// class Program
// {
//     static void Main()
//     {
//         var lruCache = new LRUCache<int, string>(3);
//         
//         lruCache.Put(1, "A");
//         lruCache.Put(2, "B");
//         lruCache.Put(3, "C");
//         
//         Console.WriteLine(lruCache.Get(1)); // A
//         lruCache.Put(4, "D");
//         
//         Console.WriteLine(lruCache.Get(2)); // default (null, not in cache)
//     }
// }
