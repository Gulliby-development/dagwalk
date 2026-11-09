using static Dagwalk.Tests.Rows;

namespace Dagwalk.Tests;

public class WhyRows
{
    [Fact]
    public void Rows()
    {
        Row(Args("why", "app", "core"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 0, "app -> lib -> core\n", "");
        Row(Args("why", "app", "util"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 0, "app -> util\n", "");
        Row(Args("why", "extra", "base"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 0, "extra -> top -> left -> base\n", "");
        Row(Args("why", "base", "extra"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 1, "", "dagwalk: no dependency path from base to extra\n");
        Row(Args("why", "left", "left"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 0, "left\n", "");
        Row(Args("why", "zz", "left"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 1, "", "dagwalk: unknown target zz\n");
        Row(Args("why", "left", "zz"), "top: left right\nleft: base\nright: base\nbase:\nextra: top\n", 1, "", "dagwalk: unknown target zz\n");
        Row(Args("why", "a", "c"), "a: b\nb: c\nc: a\nd: a\ne:\n", 0, "a -> b -> c\n", "");
        Row(Args("why", "d", "c"), "a: b\nb: c\nc: a\nd: a\ne:\n", 0, "d -> a -> b -> c\n", "");
        Row(Args("why", "a", "a"), "a: b\nb: c\nc: a\nd: a\ne:\n", 0, "a\n", "");
        Row(Args("why", "s", "t"), "s: m2 m1\nm1: t\nm2: t\nt:\n", 0, "s -> m2 -> t\n", "");
        Row(Args("why", "s", "t"), "m1: t\nm2: t\ns: m2 m1\nt:\n", 0, "s -> m1 -> t\n", "");
        Row(Args("why", "a", "z"), "a: b c\nb: d\nc: e\nd: z\ne: f\nf: z\nz:\n", 0, "a -> b -> d -> z\n", "");
    }
}
