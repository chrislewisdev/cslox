
namespace CsLox;

public class LoxFunction : ICallable
{
    private readonly Stmt.Function declaration;
    private readonly Environment closure;

    public LoxFunction(Stmt.Function declaration, Environment closure)
    {
        this.declaration = declaration;
        this.closure = closure;
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
            return r.Value;
        }

        return null;
    }

    public override string ToString() => $"<fn {declaration.Name.Lexeme}>";
}
