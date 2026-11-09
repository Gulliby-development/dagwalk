using static Dagwalk.Tests.Rows;

namespace Dagwalk.Tests;

public class LevelsRows
{
    [Fact]
    public void Rows()
    {
        Row(Args("levels"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 0, "L0: util\nL1: core\nL2: lib\nL3: app\n", "");
        Row(Args("levels"), "app: zeta alpha\nzeta:\nalpha:\n", 0, "L0: zeta alpha\nL1: app\n", "");
        Row(Args("levels"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 0, "L0: base\nL1: left right\nL2: top\nL3: extra\n", "");
        Row(Args("levels", "--from", "left"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 0, "L0: base\nL1: left\n", "");
        Row(Args("levels", "--from", "extra"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 0, "L0: base\nL1: left right\nL2: top\nL3: extra\n", "");
        Row(Args("levels"), "", 0, "", "");
        Row(Args("levels"), "a: b\nb: c\nc: a\nd: a\ne:\n", 1, "", "dagwalk: cycle blocks: a b c d\n");
        Row(Args("levels", "--from", "e"), "a: b\nb: c\nc: a\nd: a\ne:\n", 0, "L0: e\n", "");
        Row(Args("levels", "--from", "zz"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 1, "", "dagwalk: unknown target zz\n");
        Row(Args("levels"), "z: y x\ny: x\nx:\nw:\n", 0, "L0: x w\nL1: y\nL2: z\n", "");
    }
}
