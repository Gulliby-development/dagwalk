namespace Dagwalk;

/// <summary>Finds shortest dependency chains.</summary>
public static class WhyCommand
{
    public static Result Run(Options options, string stdin)
    {
        var graph = Graph.Read(stdin);
        var fromName = options.Positionals[0];
        var toName = options.Positionals[1];
        var from = graph.Lookup(fromName);
        var to = graph.Lookup(toName);

        if (from == to)
        {
            return new Result(0, $"{graph.Names[from]}\n", "");
        }

        var predecessors = new int?[graph.Names.Count];
        var visited = new bool[graph.Names.Count];
        var pending = new Queue<int>();
        visited[from] = true;
        pending.Enqueue(from);

        while (pending.Count > 0 && !visited[to])
        {
            var current = pending.Dequeue();
            foreach (var dependency in graph.Dependencies[current].Order())
            {
                if (visited[dependency])
                {
                    continue;
                }

                visited[dependency] = true;
                predecessors[dependency] = current;
                pending.Enqueue(dependency);
            }
        }

        if (!visited[to])
        {
            throw new Failure(1, $"no dependency path from {fromName} to {toName}");
        }

        var path = new List<string>();
        for (int? current = to; current is not null; current = predecessors[current.Value])
        {
            path.Add(graph.Names[current.Value]);
        }

        path.Reverse();
        return new Result(0, string.Join(" -> ", path) + "\n", "");
    }
}
