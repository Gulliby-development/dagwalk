using static Dagwalk.Tests.Rows;

namespace Dagwalk.Tests;

public class OrderRows
{
    [Fact]
    public void Rows()
    {
        Row(Args("order"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 0, "util\ncore\nlib\napp\n", "");
        Row(Args("order", "--tie", "name"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 0, "util\ncore\nlib\napp\n", "");
        Row(Args("order"), "app: zeta alpha\nzeta:\nalpha:\n", 0, "zeta\nalpha\napp\n", "");
        Row(Args("order", "--tie", "name"), "app: zeta alpha\nzeta:\nalpha:\n", 0, "alpha\nzeta\napp\n", "");
        Row(Args("order"), "# build graph\n\napp: lib  util\nlib:\tutil core\nutil:\ncore: util\n", 0, "util\ncore\nlib\napp\n", "");
        Row(Args("order"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 0, "base\nleft\nright\ntop\nextra\n", "");
        Row(Args("order", "--tie", "name"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 0, "base\nleft\nright\ntop\nextra\n", "");
        Row(Args("order", "--from", "left"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 0, "base\nleft\n", "");
        Row(Args("order", "--from", "extra", "--tie", "name"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 0, "base\nleft\nright\ntop\nextra\n", "");
        Row(Args("order", "--from", "nothere"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 1, "", "dagwalk: unknown target nothere\n");
        Row(Args("order", "--from", "base"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 0, "base\n", "");
        Row(Args("order"), "a: b\nb: c\nc: a\nd: a\ne:\n", 1, "", "dagwalk: cycle blocks: a b c d\n");
        Row(Args("order", "--from", "e"), "a: b\nb: c\nc: a\nd: a\ne:\n", 0, "e\n", "");
        Row(Args("order", "--from", "d"), "a: b\nb: c\nc: a\nd: a\ne:\n", 1, "", "dagwalk: cycle blocks: a b c d\n");
        Row(Args("order"), "a: a\n", 1, "", "dagwalk: cycle blocks: a\n");
        Row(Args("order", "--tie", "name"), "b: a\nc: a\na:\nD: a\nB: a\n", 0, "a\nB\nD\nb\nc\n", "");
    }
}
