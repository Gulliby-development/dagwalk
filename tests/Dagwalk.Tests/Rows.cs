namespace Dagwalk.Tests;

/// <summary>The helpers of the row tests: one call checks one command line.</summary>
public static class Rows
{
    /// <summary>The arguments of a command line.</summary>
    public static string[] Args(params string[] args) => args;

    /// <summary>Runs Cli.Run on the arguments with the standard input text and checks the exit status, the standard output text and the standard error text.</summary>
    public static void Row(string[] args, string stdin, int code, string expectedOut, string expectedErr)
    {
        var result = Cli.Run(args, stdin);
        Assert.Equal(expectedErr, result.Err);
        Assert.Equal(expectedOut, result.Out);
        Assert.Equal(code, result.Status);
    }

    /// <summary>Checks whether Cli.ReadsStdin says that the command line reads standard input.</summary>
    public static void Reads(string[] args, bool expected) => Assert.Equal(expected, Cli.ReadsStdin(args));
}
