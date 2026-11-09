using static Dagwalk.Tests.Rows;

namespace Dagwalk.Tests;

public class ParseRows
{
    [Fact]
    public void Rows()
    {
        Row(Args("order"), "", 0, "", "");
        Row(Args("order"), "\n# note\n   \n", 0, "", "");
        Row(Args("order"), "a: b\r\nb:\r\n", 0, "b\na\n", "");
        Row(Args("order"), "  a  :  b\tc  \nb:\nc:", 0, "b\nc\na\n", "");
        Row(Args("order"), "a:b\n", 0, "b\na\n", "");
        Row(Args("order"), "a: b\nb a\n", 1, "", "dagwalk: line 2: missing colon\n");
        Row(Args("order"), "a: b\nb:\na: c\n", 1, "", "dagwalk: line 3: duplicate target a\n");
        Row(Args("order"), "a: b\n# x\n\nb c\n", 1, "", "dagwalk: line 4: missing colon\n");
        Row(Args("order"), "a: b$c\n", 1, "", "dagwalk: line 1: bad name\n");
        Row(Args("order"), ": a\n", 1, "", "dagwalk: line 1: bad name\n");
        Row(Args("order"), "a: b:c\n", 1, "", "dagwalk: line 1: bad name\n");
        Row(Args("order"), "a: b\nnnnnnnnnnnnnnnnnnnnnnnnn: a\n", 0, "b\na\nnnnnnnnnnnnnnnnnnnnnnnnn\n", "");
        Row(Args("order"), "nnnnnnnnnnnnnnnnnnnnnnnnn: a\n", 1, "", "dagwalk: line 1: bad name\n");
        Row(Args("order"), "a: nnnnnnnnnnnnnnnnnnnnnnnnn\n", 1, "", "dagwalk: line 1: bad name\n");
        Row(Args("order"), "a.b-c_D1: x.y-z\n", 0, "x.y-z\na.b-c_D1\n", "");
        Row(Args("order"), "a: b\nc d: e\n", 1, "", "dagwalk: line 2: bad name\n");
        Row(Args("order"), "a: b b b\nb: c c\n", 0, "c\nb\na\n", "");
        Row(Args("order"), "a: b\n# last", 0, "b\na\n", "");
    }
}
