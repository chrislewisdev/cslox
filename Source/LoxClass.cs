namespace CsLox;

public class LoxClass : ICallable
{
    public readonly string Name;

    private readonly Dictionary<string, LoxFunction> methods;

    public LoxClass(string name, Dictionary<string, LoxFunction> methods)
    {
        Name = name;
        this.methods = methods;
    }

    public override string ToString() => Name;

    public int Arity => FindMethod("init")?.Arity ?? 0;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        var instance = new LoxInstance(this);

        var initialiser = FindMethod("init");
        if (initialiser != null)
        {
            initialiser.Bind(instance).Call(interpreter, arguments);
        }

        return instance;
    }

    public LoxFunction FindMethod(string name)
    {
        if (methods.ContainsKey(name)) return methods[name];

        return null;
    }
}

