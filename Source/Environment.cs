namespace CsLox;

public class Environment
{
    public class Uninitialised {}

    private readonly Environment enclosing = null;
    private readonly Dictionary<string, object> values = new();

    public Environment() {}

    public Environment(Environment enclosing)
    {
        this.enclosing = enclosing;
    }

    public object Get(Token name)
    {
        if (values.ContainsKey(name.Lexeme))
        {
            var @value = values[name.Lexeme];
            if (@value is Uninitialised) throw new RuntimeError(name, $"Uninitialised variable {name.Lexeme}");
            return @value;
        }

        if (enclosing != null) return enclosing.Get(name);

        throw new RuntimeError(name, $"Undefined variable {name.Lexeme}.");
    }

    public void Define(string name, object @value)
    {
        values[name] = @value;
    }

    public void Assign(Token name, object @value)
    {
        if (values.ContainsKey(name.Lexeme))
        {
            values[name.Lexeme] = @value;
            return;
        }

        if (enclosing != null)
        {
            enclosing.Assign(name, @value);
            return;
        }

        throw new RuntimeError(name, $"Assigning to undefined variable {name.Lexeme}");
    }
}

