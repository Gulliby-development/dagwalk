using static Dagwalk.Tests.Rows;

namespace Dagwalk.Tests;

public class ReadsRows
{
    [Fact]
    public void Rows()
    {
        Reads(Args("order"), true);
        Reads(Args("levels", "--from", "a"), true);
        Reads(Args("why", "a", "b"), true);
        Reads(Args("help"), false);
        Reads(Args(), false);
        Reads(Args("frob"), false);
        Reads(Args("order", "--bogus"), true);
        Reads(Args("--version"), false);
    }
}
