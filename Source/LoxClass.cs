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

    public int Arity => 0;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return new LoxInstance(this);
    }

    public LoxFunction FindMethod(string name)
    {
        if (methods.ContainsKey(name)) return methods[name];

        return null;
    }
}

