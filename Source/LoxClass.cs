namespace CsLox;

public class LoxClass
{
    public readonly string Name;

    public LoxClass(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;
}

