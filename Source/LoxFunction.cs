
namespace CsLox;

public class LoxFunction : ICallable
{
    private readonly Stmt.Function declaration;
    private readonly Environment closure;
    private readonly bool isInitialiser;

    public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitialiser)
    {
        this.declaration = declaration;
        this.closure = closure;
        this.isInitialiser = isInitialiser;
    }

    public int Arity => declaration.Parameters.Count;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        var environment = new Environment(closure);
        for (var i = 0; i < declaration.Parameters.Count; i++)
        {
            environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
        }

        try
        {
            interpreter.ExecuteBlock(declaration.Body, environment);
        }
        catch (Return r)
        {
            if (isInitialiser) return closure.GetAt(0, "this");
            return r.Value;
        }

        if (isInitialiser) return closure.GetAt(0, "this");
        return null;
    }

    public LoxFunction Bind(LoxInstance instance)
    {
        var environment = new Environment(closure);
        environment.Define("this", instance);
        return new LoxFunction(declaration, environment, isInitialiser);
    }

    public override string ToString() => $"<fn {declaration.Name.Lexeme}>";
}
