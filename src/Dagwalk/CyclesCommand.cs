namespace Dagwalk;

/// <summary>Runs the cycles command over a dependency graph.</summary>
public static class CyclesCommand
{
    public static Result Run(Options options, string stdin)
    {
        var graph = Graph.Read(stdin);
        var groups = graph.CycleGroups();
        var output = groups.Count == 0
            ? ""
            : string.Join("\n", groups.Select(group => string.Join(" ", group.Select(id => graph.Names[id])))) + "\n";

        return new Result(0, output, "");
    }
}
