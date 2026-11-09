using static Dagwalk.Tests.Rows;

namespace Dagwalk.Tests;

public class CheckRows
{
    [Fact]
    public void Rows()
    {
        Row(Args("check"), "app: lib util\nlib: util core\nutil:\ncore: util\n", 0, "ok\n", "");
        Row(Args("check"), "", 0, "ok\n", "");
        Row(Args("check"), "a: b\nb: c\n", 1, "unknown dependency: b -> c\n", "");
        Row(Args("check"), "a: b\nb: c\nc: a\nd: a\ne:\n", 1, "cycle: a b c\n", "");
        Row(Args("check"), "a: x y\nb: x\nx:\nc: a\n", 1, "unknown dependency: a -> y\n", "");
        Row(Args("check"), "a: b\nb: a\nc: d\nd:\ne: f g\n", 1, "unknown dependency: e -> f\nunknown dependency: e -> g\ncycle: a b\n", "");
        Row(Args("check"), "a: a\n", 1, "cycle: a\n", "");
        Row(Args("check"), "a: b b\nb:\n", 0, "ok\n", "");
        Row(Args("check"), "a: b\nb: c\nc: b\nd: e\ne: zzz\n", 1, "unknown dependency: e -> zzz\ncycle: b c\n", "");
        Row(Args("check"), "a: b c\nb:\nc:\n", 0, "ok\n", "");
    }
}
