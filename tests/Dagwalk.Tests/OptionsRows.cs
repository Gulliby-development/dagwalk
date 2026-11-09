using static Dagwalk.Tests.Rows;

namespace Dagwalk.Tests;

public class OptionsRows
{
    [Fact]
    public void Rows()
    {
        Row(Args("order", "--bogus"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: unknown option --bogus\n");
        Row(Args("order", "--from"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: missing value for --from\n");
        Row(Args("order", "--tie"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: missing value for --tie\n");
        Row(Args("order", "--tie", "random"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: bad value for --tie: random\n");
        Row(Args("order", "--tie", "Name"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: bad value for --tie: Name\n");
        Row(Args("order", "--from", "app", "--from", "lib"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: option given twice: --from\n");
        Row(Args("order", "--tie", "name", "--tie", "name"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: option given twice: --tie\n");
        Row(Args("levels", "--tie", "name"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: option --tie is not used by levels\n");
        Row(Args("why", "--from", "a", "app", "util"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: option --from is not used by why\n");
        Row(Args("why", "app"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: why needs FROM and TO\n");
        Row(Args("why", "app", "lib", "util"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: why needs FROM and TO\n");
        Row(Args("order", "--from", "--tie"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 1, "", "dagwalk: unknown target --tie\n");
        Row(Args("order", "-x"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: order takes no arguments\n");
        Row(Args("levels", "--"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: unknown option --\n");
        Row(Args("order", "--tie", "x", "--tie", "y"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: bad value for --tie: x\n");
        Row(Args("check", "--tie", "name"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 2, "", "dagwalk: option --tie is not used by check\n");
    }
}
