namespace Dagwalk;

/// <summary>Parses command lines and dispatches registered commands.</summary>
public static class Cli
{
    public static bool ReadsStdin(string[] args) =>
        args.Length > 0 && Registry.Commands.Any(command => command.Name == args[0]);

    public static Result Run(string[] args, string stdin)
    {
        if (args.Length == 0)
        {
            return new Result(2, "", Registry.Usage);
        }

        var first = args[0];
        if (first is "help" or "-h" or "--help")
        {
            return new Result(0, Registry.Usage, "");
        }

        if (first == "--version")
        {
            return new Result(0, "dagwalk 0.1.0\n", "");
        }

        var command = Registry.Commands.FirstOrDefault(item => item.Name == first);
        if (command is null)
        {
            return Error(2, $"unknown command {first}");
        }

        try
        {
            var options = ParseOptions(command, args);
            return command.Run(options, stdin);
        }
        catch (Failure failure)
        {
            return Error(failure.Status, failure.Message);
        }
    }

    private static Options ParseOptions(Command command, string[] args)
    {
        string? from = null;
        var tie = "input";
        var seen = new HashSet<string>();
        var positionals = new List<string>();

        for (var index = 1; index < args.Length; index++)
        {
            var word = args[index];
            if (!word.StartsWith("--", StringComparison.Ordinal))
            {
                positionals.Add(word);
                continue;
            }

            if (word is not ("--from" or "--tie"))
            {
                throw new Failure(2, $"unknown option {word}");
            }

            if (!command.Allowed.Contains(word))
            {
                throw new Failure(2, $"option {word} is not used by {command.Name}");
            }

            if (index + 1 >= args.Length)
            {
                throw new Failure(2, $"missing value for {word}");
            }

            if (!seen.Add(word))
            {
                throw new Failure(2, $"option given twice: {word}");
            }

            var value = args[++index];
            if (word == "--from")
            {
                from = value;
            }
            else if (value is "input" or "name")
            {
                tie = value;
            }
            else
            {
                throw new Failure(2, $"bad value for --tie: {value}");
            }
        }

        if (command.Name == "why")
        {
            if (positionals.Count != 2)
            {
                throw new Failure(2, "why needs FROM and TO");
            }
        }
        else if (positionals.Count != 0)
        {
            throw new Failure(2, $"{command.Name} takes no arguments");
        }

        return new Options(from, tie, positionals.AsReadOnly());
    }

    private static Result Error(int status, string message) =>
        new(status, "", $"dagwalk: {message}\n");
}
