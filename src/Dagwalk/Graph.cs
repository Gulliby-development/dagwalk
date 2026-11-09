using System.Collections.Frozen;
using System.Globalization;

namespace Dagwalk;

/// <summary>The parsed dependency graph and its shared selection and traversal operations.</summary>
public sealed class Graph
{
    private readonly IReadOnlyDictionary<string, int> _ids;

    private Graph(
        IReadOnlyList<string> names,
        IReadOnlyList<IReadOnlyList<int>> dependencies,
        IReadOnlySet<int> explicitTargets,
        IReadOnlyList<int> declarationOrder,
        IReadOnlyDictionary<string, int> ids)
    {
        Names = names;
        Dependencies = dependencies;
        ExplicitTargets = explicitTargets;
        DeclarationOrder = declarationOrder;
        _ids = ids;
    }

    public IReadOnlyList<string> Names { get; }
    public IReadOnlyList<IReadOnlyList<int>> Dependencies { get; }
    public IReadOnlySet<int> ExplicitTargets { get; }
    public IReadOnlyList<int> DeclarationOrder { get; }

    public static Graph Read(string stdin)
    {
        var names = new List<string>();
        var ids = new Dictionary<string, int>(StringComparer.Ordinal);
        var adjacency = new List<List<int>>();
        var explicitTargets = new HashSet<int>();
        var declarationOrder = new List<int>();
        var lineNumber = 0;
        var start = 0;

        while (start < stdin.Length)
        {
            var lf = stdin.IndexOf('\n', start);
            var terminated = lf >= 0;
            var end = terminated ? lf : stdin.Length;
            var line = stdin[start..end];
            if (terminated && line.EndsWith('\r'))
            {
                line = line[..^1];
            }

            lineNumber++;
            ParseLine(line, lineNumber, names, ids, adjacency, explicitTargets, declarationOrder);
            if (!terminated)
            {
                break;
            }

            start = end + 1;
        }

        var frozenDependencies = adjacency
            .Select(row => (IReadOnlyList<int>)Array.AsReadOnly(row.ToArray()))
            .ToArray();
        return new Graph(
            Array.AsReadOnly(names.ToArray()),
            Array.AsReadOnly(frozenDependencies),
            explicitTargets.ToFrozenSet(),
            Array.AsReadOnly(declarationOrder.ToArray()),
            new System.Collections.ObjectModel.ReadOnlyDictionary<string, int>(ids));
    }

    public int Lookup(string name) =>
        _ids.TryGetValue(name, out var id) ? id : throw new Failure(1, $"unknown target {name}");

    public IReadOnlySet<int> Select(string? from)
    {
        if (from is null)
        {
            return Enumerable.Range(0, Names.Count).ToFrozenSet();
        }

        var selected = new HashSet<int>();
        var pending = new Stack<int>();
        pending.Push(Lookup(from));
        while (pending.Count > 0)
        {
            var id = pending.Pop();
            if (!selected.Add(id))
            {
                continue;
            }

            foreach (var dependency in Dependencies[id])
            {
                pending.Push(dependency);
            }
        }

        return selected.ToFrozenSet();
    }

    public IReadOnlyList<int> TopologicalOrder(IReadOnlySet<int> selection, string tie)
    {
        var ready = new SortedSet<int>(Comparer<int>.Create((left, right) =>
        {
            var comparison = tie == "name"
                ? StringComparer.Ordinal.Compare(Names[left], Names[right])
                : left.CompareTo(right);
            return comparison != 0 ? comparison : left.CompareTo(right);
        }));
        var remainingDependencies = new int[Names.Count];
        var dependents = Enumerable.Range(0, Names.Count).Select(_ => new List<int>()).ToArray();

        foreach (var id in selection)
        {
            foreach (var dependency in Dependencies[id])
            {
                if (selection.Contains(dependency))
                {
                    remainingDependencies[id]++;
                    dependents[dependency].Add(id);
                }
            }

            if (remainingDependencies[id] == 0)
            {
                ready.Add(id);
            }
        }

        var result = new List<int>(selection.Count);
        while (ready.Count > 0)
        {
            var id = ready.Min;
            ready.Remove(id);
            result.Add(id);
            foreach (var dependent in dependents[id])
            {
                remainingDependencies[dependent]--;
                if (remainingDependencies[dependent] == 0)
                {
                    ready.Add(dependent);
                }
            }
        }

        if (result.Count != selection.Count)
        {
            var blocked = selection
                .Where(id => remainingDependencies[id] > 0)
                .Select(id => Names[id])
                .Order(StringComparer.Ordinal);
            throw new Failure(1, $"cycle blocks: {string.Join(" ", blocked)}");
        }

        return Array.AsReadOnly(result.ToArray());
    }

    public IReadOnlyList<IReadOnlyList<int>> CycleGroups()
    {
        var visited = new bool[Names.Count];
        var finishOrder = new List<int>(Names.Count);

        for (var start = 0; start < Names.Count; start++)
        {
            if (visited[start])
            {
                continue;
            }

            visited[start] = true;
            var pending = new Stack<(int Node, int NextDependency)>();
            pending.Push((start, 0));
            while (pending.Count > 0)
            {
                var (node, nextDependency) = pending.Pop();
                if (nextDependency < Dependencies[node].Count)
                {
                    pending.Push((node, nextDependency + 1));
                    var dependency = Dependencies[node][nextDependency];
                    if (!visited[dependency])
                    {
                        visited[dependency] = true;
                        pending.Push((dependency, 0));
                    }
                }
                else
                {
                    finishOrder.Add(node);
                }
            }
        }

        var dependents = Enumerable.Range(0, Names.Count).Select(_ => new List<int>()).ToArray();
        for (var node = 0; node < Names.Count; node++)
        {
            foreach (var dependency in Dependencies[node])
            {
                dependents[dependency].Add(node);
            }
        }

        Array.Fill(visited, false);
        var groups = new List<IReadOnlyList<int>>();
        for (var index = finishOrder.Count - 1; index >= 0; index--)
        {
            var start = finishOrder[index];
            if (visited[start])
            {
                continue;
            }

            var component = new List<int>();
            var pending = new Stack<int>();
            pending.Push(start);
            visited[start] = true;
            while (pending.Count > 0)
            {
                var node = pending.Pop();
                component.Add(node);
                foreach (var dependent in dependents[node])
                {
                    if (!visited[dependent])
                    {
                        visited[dependent] = true;
                        pending.Push(dependent);
                    }
                }
            }

            component.Sort();
            if (component.Count > 1 || Dependencies[start].Contains(start))
            {
                groups.Add(Array.AsReadOnly(component.ToArray()));
            }
        }

        groups.Sort((left, right) => left[0].CompareTo(right[0]));
        return Array.AsReadOnly(groups.ToArray());
    }

    private static void ParseLine(
        string original,
        int lineNumber,
        List<string> names,
        Dictionary<string, int> ids,
        List<List<int>> adjacency,
        HashSet<int> explicitTargets,
        List<int> declarationOrder)
    {
        var line = TrimSpacesAndTabs(original);
        if (line.Length == 0 || line[0] == '#')
        {
            return;
        }

        var colon = line.IndexOf(':');
        if (colon < 0)
        {
            throw LineFailure(lineNumber, "missing colon");
        }

        var target = TrimSpacesAndTabs(line[..colon]);
        var dependencyText = line[(colon + 1)..];
        var dependencies = SplitWords(dependencyText);
        if (!IsName(target) || dependencies.Any(dependency => !IsName(dependency)))
        {
            throw LineFailure(lineNumber, "bad name");
        }

        if (explicitTargets.Contains(ids.TryGetValue(target, out var existing) ? existing : -1))
        {
            throw LineFailure(lineNumber, $"duplicate target {target}");
        }

        var targetId = Intern(target, names, ids, adjacency);
        explicitTargets.Add(targetId);
        declarationOrder.Add(targetId);
        var seen = new HashSet<int>();
        foreach (var dependency in dependencies)
        {
            var dependencyId = Intern(dependency, names, ids, adjacency);
            if (seen.Add(dependencyId))
            {
                adjacency[targetId].Add(dependencyId);
            }
        }
    }

    private static int Intern(string name, List<string> names, Dictionary<string, int> ids, List<List<int>> adjacency)
    {
        if (ids.TryGetValue(name, out var id))
        {
            return id;
        }

        id = names.Count;
        names.Add(name);
        ids.Add(name, id);
        adjacency.Add(new List<int>());
        return id;
    }

    private static string[] SplitWords(string text) =>
        text.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

    private static string TrimSpacesAndTabs(string text) => text.Trim(' ', '\t');

    private static bool IsName(string name)
    {
        if (name.Length is < 1 or > 24)
        {
            return false;
        }

        foreach (var character in name)
        {
            if (!(character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '_' or '.' or '-'))
            {
                return false;
            }
        }

        return true;
    }

    private static Failure LineFailure(int lineNumber, string reason) =>
        new(1, $"line {lineNumber.ToString(CultureInfo.InvariantCulture)}: {reason}");
}
