using ConsoleApp1;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("app.log")
    .CreateLogger();
Console.OutputEncoding = Encoding.Unicode;

while (true)
{
    SameKeyProcessor.FindSameKeys();
    // QuestProcessor.Run();
}
