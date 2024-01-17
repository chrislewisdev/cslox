namespace CsLox;

public class LoxInstance
{
    private LoxClass klass;
    private readonly Dictionary<string, object> fields = new();

    public LoxInstance(LoxClass klass)
    {
        this.klass = klass;
    }

    public override string ToString() => $"{klass.Name} instance";

    public object Get(Token name)
    {
        if (fields.ContainsKey(name.Lexeme))
        {
            return fields[name.Lexeme];
        }

        var method = klass.FindMethod(name.Lexeme);
        if (method != null) return method.Bind(this);

        throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.");
    }

    public void Set(Token name, object @value)
    {
        fields[name.Lexeme] = @value;
    }
}

