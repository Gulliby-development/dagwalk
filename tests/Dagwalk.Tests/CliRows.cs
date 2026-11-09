using static Dagwalk.Tests.Rows;

namespace Dagwalk.Tests;

public class CliRows
{
    [Fact]
    public void Rows()
    {
        Row(Args(), "", 2, "", "usage: dagwalk order [--from NAME] [--tie input|name]\n       dagwalk levels [--from NAME]\n       dagwalk why FROM TO\n       dagwalk cycles\n       dagwalk check\n");
        Row(Args("help"), "", 0, "usage: dagwalk order [--from NAME] [--tie input|name]\n       dagwalk levels [--from NAME]\n       dagwalk why FROM TO\n       dagwalk cycles\n       dagwalk check\n", "");
        Row(Args("-h", "extra"), "", 0, "usage: dagwalk order [--from NAME] [--tie input|name]\n       dagwalk levels [--from NAME]\n       dagwalk why FROM TO\n       dagwalk cycles\n       dagwalk check\n", "");
        Row(Args("--help"), "", 0, "usage: dagwalk order [--from NAME] [--tie input|name]\n       dagwalk levels [--from NAME]\n       dagwalk why FROM TO\n       dagwalk cycles\n       dagwalk check\n", "");
        Row(Args("--version"), "", 0, "dagwalk 0.1.0\n", "");
        Row(Args("--version", "x"), "", 0, "dagwalk 0.1.0\n", "");
        Row(Args("frob"), "", 2, "", "dagwalk: unknown command frob\n");
        Row(Args(""), "", 2, "", "dagwalk: unknown command \n");
        Row(Args("Order"), "", 2, "", "dagwalk: unknown command Order\n");
        Row(Args("--tie"), "", 2, "", "dagwalk: unknown command --tie\n");
        Row(Args("order", "extra"), "", 2, "", "dagwalk: order takes no arguments\n");
        Row(Args("cycles", "a"), "", 2, "", "dagwalk: cycles takes no arguments\n");
        Row(Args("check", "--from", "a"), "", 2, "", "dagwalk: option --from is not used by check\n");
    }
}
