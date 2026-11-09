namespace Dagwalk;

/// <summary>What one command line produced: the exit status and the text for standard output and standard error.</summary>
public sealed record Result(int Status, string Out, string Err);

/// <summary>Thrown by a command or the graph reader: the exit status and the message without "dagwalk: " and the line feed.</summary>
public sealed class Failure(int status, string message) : Exception(message)
{
    public int Status { get; } = status;
}

/// <summary>The options of a command line: --from (null when absent), --tie (input unless given) and the positional arguments in order.</summary>
public sealed record Options(string? From, string Tie, IReadOnlyList<string> Positionals);

/// <summary>A command: its name, the options it accepts and its handler. The handler gets the options and the standard input and returns the result of the command or throws Failure.</summary>
public sealed record Command(string Name, IReadOnlySet<string> Allowed, Func<Options, string, Result> Run);
