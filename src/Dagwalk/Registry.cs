namespace Dagwalk;

/// <summary>The usage text and the table of commands.</summary>
public static class Registry
{
    public const string Usage =
        "usage: dagwalk order [--from NAME] [--tie input|name]\n" +
        "       dagwalk levels [--from NAME]\n" +
        "       dagwalk why FROM TO\n" +
        "       dagwalk cycles\n" +
        "       dagwalk check\n";

    public static readonly IReadOnlyList<Command> Commands =
    [
        new Command("order", new HashSet<string> { "--from", "--tie" }, OrderCommand.Run),
        new Command("levels", new HashSet<string> { "--from" }, LevelsCommand.Run),
        new Command("why", new HashSet<string>(), WhyCommand.Run),
        new Command("cycles", new HashSet<string>(), CyclesCommand.Run),
        new Command("check", new HashSet<string>(), CheckCommand.Run),
    ];
}
