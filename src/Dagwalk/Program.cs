using System.Text;
using Dagwalk;

var utf8 = new UTF8Encoding(false);
var stdin = Cli.ReadsStdin(args) ? new StreamReader(Console.OpenStandardInput(), utf8).ReadToEnd() : "";
var result = Cli.Run(args, stdin);
Write(Console.OpenStandardOutput(), result.Out);
Write(Console.OpenStandardError(), result.Err);
return result.Status;

void Write(Stream stream, string text)
{
    var bytes = utf8.GetBytes(text);
    stream.Write(bytes, 0, bytes.Length);
    stream.Flush();
}
