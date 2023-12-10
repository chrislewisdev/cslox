namespace CsLox;

public class Environment
{
    private readonly Dictionary<string, object> values = new();

    public object Get(Token name)
    {
        if (values.ContainsKey(name.Lexeme))
        {
            return values[name.Lexeme];
        }

        throw new RuntimeError(name, $"Undefined variable {name.Lexeme}.");
    }

    public void Define(string name, object @value)
    {
        values[name] = @value;
    }
}

