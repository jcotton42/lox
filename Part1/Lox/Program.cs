namespace Lox;

public static class Lox
{
    private static bool hadError = false;

    public static int Main(string[] args)
    {
        switch (args)
        {
            case []:
                RunPrompt();
                return 0;
            case [var path]:
                return RunFile(path);
            default:
                Console.Out.WriteLine("Usage: lox [script]");
                return 64;
        }
    }

    private static int RunFile(string path)
    {
        Run(File.ReadAllText(path));
        return hadError ? 65 : 0;
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line is null)
            {
                break;
            }

            Run(line);
            hadError = false;
        }
    }

    private static void Run(string source)
    {
        foreach (var token in new Scanner(source).ScanTokens())
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message) => Report(line, "", message);

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        hadError = true;
    }
}
