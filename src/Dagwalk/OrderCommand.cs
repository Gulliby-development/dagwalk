namespace Dagwalk;

/// <summary>Runs the order command over the selected dependency graph.</summary>
public static class OrderCommand
{
    public static Result Run(Options options, string stdin)
    {
        var graph = Graph.Read(stdin);
        var selection = graph.Select(options.From);
        var order = graph.TopologicalOrder(selection, options.Tie);
        var output = order.Count == 0
            ? ""
            : string.Join("\n", order.Select(id => graph.Names[id])) + "\n";

        return new Result(0, output, "");
    }
}
