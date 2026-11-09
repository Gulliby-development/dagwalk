namespace Dagwalk;

/// <summary>Reports undeclared dependencies and cyclic components in a dependency graph.</summary>
public static class CheckCommand
{
    public static Result Run(Options options, string stdin)
    {
        var graph = Graph.Read(stdin);
        var reports = new List<string>();

        foreach (var target in graph.DeclarationOrder)
        {
            foreach (var dependency in graph.Dependencies[target])
            {
                if (!graph.ExplicitTargets.Contains(dependency))
                {
                    reports.Add($"unknown dependency: {graph.Names[target]} -> {graph.Names[dependency]}");
                }
            }
        }

        foreach (var group in graph.CycleGroups())
        {
            reports.Add($"cycle: {string.Join(" ", group.Select(id => graph.Names[id]))}");
        }

        return reports.Count == 0
            ? new Result(0, "ok\n", "")
            : new Result(1, string.Join("\n", reports) + "\n", "");
    }
}
