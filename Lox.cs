namespace cslox;
class Lox
{
    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cslox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            //RunFile(args[0]);
        }
        else
        {
            //RunPrompt();
        }
    }
}
