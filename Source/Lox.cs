﻿namespace CsLox;

class Lox
{
    private static bool hadError = false;

    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cslox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string filename)
    {
        Run(File.ReadAllText(filename));
        if (hadError) Environment.Exit(65);
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) break;
            Run(line);
            hadError = false;
        }
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var expression = parser.Parse();

        if (hadError) return;

        Console.WriteLine(new AstPrinter().Print(expression));
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
            Report(token.Line, " at end", message);
        else
            Report(token.Line, $"at '{token.Lexeme}'", message);
    }

    private static void Report(int line, string @where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {@where}: {message}");
        hadError = true;
    }
}
