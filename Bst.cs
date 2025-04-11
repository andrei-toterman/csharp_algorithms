using System.Collections;

namespace csharp;

public sealed class Bst<T> where T : IComparable<T>
{
    public static void Example()
    {
        var bst = new Bst<char>(['f', 'g', 'i', 'h', 'b', 'a', 'd', 'c', 'e']);
        Console.WriteLine("Pre-Order   : " + string.Join(", ", bst.PreOrder()));
        Console.WriteLine("In-Order    : " + string.Join(", ", bst.InOrder()));
        Console.WriteLine("In-Order2   : " + string.Join(", ", bst.InOrder2()));
        Console.WriteLine("Post-Order  : " + string.Join(", ", bst.PostOrder()));
        Console.WriteLine("Post-Order2 : " + string.Join(", ", bst.PostOrder2()));
        Console.WriteLine("Level-Order : " + string.Join(", ", bst.LevelOrder()));
    }

    private class Node
    {
        public required T Data { get; init; }
        public Node? Less, More;
    }

    private Node? _root;

    private Bst(Node? node)
    {
        _root = node;
    }

    public Bst(IEnumerable<T>? enumerable = null)
    {
        foreach (var element in enumerable ?? []) Add(element);
    }

    public (Bst<T>, Bst<T>) GetParents(T t1, T t2)
    {
        if (_root is null) return (new Bst<T>(node: null), new Bst<T>(node: null));
        var node1 = t1.CompareTo(_root.Data) switch
        {
            > 0 => _root.More,
            < 0 => _root.Less,
            _ => _root,
        };

        var node2 = t2.CompareTo(_root.Data) switch
        {
            > 0 => _root.More,
            < 0 => _root.Less,
            _ => _root,
        };

        return (new Bst<T>(node1), new Bst<T>(node2));
    }

    private void Add(T data)
    {
        ref var node = ref _root;
        while (node is not null)
        {
            switch (data.CompareTo(node.Data))
            {
                case 0: return;
                case < 0: node = ref node.Less; break;
                case > 0: node = ref node.More; break;
            }
        }

        node = new Node { Data = data };
    }

    public bool Contains(T data)
    {
        var node = _root;
        while (node is not null)
        {
            switch (data.CompareTo(node.Data))
            {
                case 0: return true;
                case < 0: node = node.Less; break;
                case > 0: node = node.More; break;
            }
        }

        return false;
    }

    public IEnumerable<T> PreOrder()
    {
        return Traverse(_root);

        IEnumerable<T> Traverse(Node? node)
        {
            if (node is null) yield break;
            yield return node.Data;
            foreach (var value in Traverse(node.Less)) yield return value;
            foreach (var value in Traverse(node.More)) yield return value;
        }
    }

    public IEnumerable<T> InOrder()
    {
        return Traverse(_root);

        IEnumerable<T> Traverse(Node? node)
        {
            if (node is null) yield break;
            foreach (var value in Traverse(node.Less)) yield return value;
            yield return node.Data;
            foreach (var value in Traverse(node.More)) yield return value;
        }
    }

    public IEnumerable<T> InOrder2()
    {
        var stack = new Stack<Node>();
        var current = _root;

        while (true)
        {
            if (current is not null)
            {
                stack.Push(current);
                current = current.Less;
                continue;
            }

            if (!stack.TryPop(out var node)) break;

            yield return node.Data;
            current = node.More;
        }
    }

    public IEnumerable<T> PostOrder2()
    {
        var stack = new Stack<Node>();
        var current = _root;
        Node? lastVisited = null;

        while (true)
        {
            if (current is not null)
            {
                stack.Push(current);
                current = current.Less;
                continue;
            }

            if (!stack.TryPop(out var node)) break;
            if (node.More == null || node.More == lastVisited)
            {
                yield return node.Data;
                lastVisited = node;
                continue;
            }

            stack.Push(node);
            current = node.More;
        }
    }

    public IEnumerable<T> PostOrder()
    {
        return Traverse(_root);

        IEnumerable<T> Traverse(Node? node)
        {
            if (node is null) yield break;
            foreach (var value in Traverse(node.Less)) yield return value;
            foreach (var value in Traverse(node.More)) yield return value;
            yield return node.Data;
        }
    }

    public IEnumerable<T> LevelOrder()
    {
        var queue = new Queue<Node?>([_root]);
        while (queue.TryDequeue(out var node))
        {
            if (node is null) continue;
            yield return node.Data;
            queue.Enqueue(node.Less);
            queue.Enqueue(node.More);
        }
    }
}
