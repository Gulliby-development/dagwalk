using System.Globalization;
using System.Text;

namespace Dagwalk;

/// <summary>Groups selected dependency nodes by their dependency height.</summary>
public static class LevelsCommand
{
    public static Result Run(Options options, string stdin)
    {
        var graph = Graph.Read(stdin);
        var selection = graph.Select(options.From);
        var order = graph.TopologicalOrder(selection, "input");
        if (order.Count == 0)
        {
            return new Result(0, "", "");
        }

        var heights = new int[graph.Names.Count];
        var highestLevel = 0;
        foreach (var id in order)
        {
            foreach (var dependency in graph.Dependencies[id])
            {
                if (selection.Contains(dependency))
                {
                    heights[id] = Math.Max(heights[id], heights[dependency] + 1);
                }
            }

            highestLevel = Math.Max(highestLevel, heights[id]);
        }

        var groups = Enumerable.Range(0, highestLevel + 1)
            .Select(_ => new List<string>())
            .ToArray();
        for (var id = 0; id < graph.Names.Count; id++)
        {
            if (selection.Contains(id))
            {
                groups[heights[id]].Add(graph.Names[id]);
            }
        }

        var output = new StringBuilder();
        for (var level = 0; level < groups.Length; level++)
        {
            output.Append('L');
            output.Append(level.ToString(CultureInfo.InvariantCulture));
            output.Append(": ");
            output.Append(string.Join(" ", groups[level]));
            output.Append('\n');
        }

        return new Result(0, output.ToString(), "");
    }
}
