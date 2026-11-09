using static Dagwalk.Tests.Rows;

namespace Dagwalk.Tests;

public class CyclesRows
{
    [Fact]
    public void Rows()
    {
        Row(Args("cycles"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 0, "", "");
        Row(Args("cycles"), "", 0, "", "");
        Row(Args("cycles"), "a: b\nb: c\nc: a\nd: a\ne:\n", 0, "a b c\n", "");
        Row(Args("cycles"), "a: a\n", 0, "a\n", "");
        Row(Args("cycles"), "x: y\ny: x\np: q\nq: r\nr: p\ns: s\n", 0, "x y\np q r\ns\n", "");
        Row(Args("cycles"), "a: b\nb: a c\nc: d\nd: c\ne: a\n", 0, "a b\nc d\n", "");
        Row(Args("cycles"), "z: y\ny: z\na: b\nb: a\n", 0, "z y\na b\n", "");
        Row(Args("cycles", "x"), "a: b\nb: c\nc: a\nd: a\ne:\n", 2, "", "dagwalk: cycles takes no arguments\n");
    }
}
