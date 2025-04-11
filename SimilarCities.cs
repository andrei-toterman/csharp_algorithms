using System.Collections.Immutable;

namespace csharp;
//https://www.hackerrank.com/contests/booking-hackathon/challenges/similar-destinations

public static class SimilarCities
{
    public static void Example()
    {
        const bool fromFile = true;
        var inputStream = fromFile ? File.OpenText("input.txt") : Console.In;

        var nCommonTags = int.Parse(inputStream.ReadLine()!);
        var destinations = new List<Destination>();
        while (inputStream.ReadLine() is { } line)
        {
            var colonIndex = line.IndexOf(':');
            var name = line[..colonIndex];
            var tags = line[(colonIndex + 1)..].Split(',').ToImmutableHashSet();
            destinations.Add(new Destination(name, new HashableSet(tags)));
        }

        var solutions = new Dictionary<HashableSet, HashSet<string>>();
        foreach (var destination in destinations)
        {
            solutions.TryAdd(destination.Tags, []);
            solutions[destination.Tags].Add(destination.Name);
        }


        int lastCount;
        var currentSolutions = solutions;
        var workDone = new HashSet<HashableSet>();
        do
        {
            lastCount = solutions.Count;
            var newSolutions = new Dictionary<HashableSet, HashSet<string>>();
            var namesToCompare = new HashSet<Destination>(destinations);
            foreach (var (tags, names) in currentSolutions.ToList())
            {
                foreach (var destination in namesToCompare)
                {
                    var workDoneKey = new HashableSet([..names, destination.Name]);
                    if (!workDone.Add(workDoneKey)) continue;

                    if (names.Contains(destination.Name)) continue;
                    // var commonTags = names.Count == 1
                    // ? commonTagsForTwo[new NamesPair(names.First(), destination.Name)]
                    // : new HashableSet(tags.Set.Intersect(destination.Tags.Set));
                    var commonTags = new HashableSet(tags.Set.Intersect(destination.Tags.Set));
                    if (commonTags.Set.Count < nCommonTags) continue;

                    newSolutions.TryAdd(commonTags, [..names]);
                    newSolutions[commonTags].Add(destination.Name);
                }

                if (names.Count == 1)
                    namesToCompare.RemoveWhere(d => names.First() == d.Name);
            }

            currentSolutions = newSolutions;

            foreach (var (tags, names) in newSolutions)
            {
                if (!solutions.TryAdd(tags, names))
                    solutions[tags].UnionWith(names);
            }
        } while (lastCount != solutions.Count);

        var trimmedSolutions = from solution in solutions
            let tags = solution.Key
            let names = solution.Value
            let namesString = string.Join(',', names.Order())
            where names.Count > 1 && tags.Set.Count >= nCommonTags
            orderby tags.Set.Count descending, namesString
            select new { namesString, tags };

        foreach (var solution in trimmedSolutions) Console.WriteLine($"{solution.namesString}:{solution.tags}");
    }
}

internal sealed record Destination(string Name, HashableSet Tags);

internal sealed class NamesPair
{
    private readonly string _name1;
    private readonly string _name2;
    private readonly int _hashCode;

    public NamesPair(string name1, string name2)
    {
        var comp = string.CompareOrdinal(name1, name2) < 0;
        _name1 = comp ? name1 : name2;
        _name2 = comp ? name2 : name1;
        var hasher = new HashCode();
        hasher.Add(_name1);
        hasher.Add(_name2);
        _hashCode = hasher.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        var properties = (NamesPair namesPair) => (namesPair._name1, namesPair._name2);

        return obj is NamesPair other && properties(this) == properties(other);
    }

    public override int GetHashCode() => _hashCode;
}

internal sealed class HashableSet
{
    public ImmutableHashSet<string> Set { get; }
    private readonly int _hashCode;

    public HashableSet(ImmutableHashSet<string> items)
    {
        Set = items;
        _hashCode = 0;
        foreach (var item in Set) _hashCode ^= item.GetHashCode();
    }

    public override int GetHashCode() => _hashCode;

    public override bool Equals(object? obj) => obj is HashableSet other && Set.SequenceEqual(other.Set);

    public override string ToString() => string.Join(',', Set.Order());
}
