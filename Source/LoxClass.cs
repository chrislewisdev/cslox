namespace CsLox;

public class LoxClass : ICallable
{
    public readonly string Name;

    public LoxClass(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;

    public int Arity => 0;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return new LoxInstance(this);
    }
}

