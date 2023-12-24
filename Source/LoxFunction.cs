
namespace CsLox;

public class LoxFunction : ICallable
{
    private readonly Stmt.Function declaration;

    public LoxFunction(Stmt.Function declaration)
    {
        this.declaration = declaration;
    }

    public int Arity => declaration.Parameters.Count;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        var environment = new Environment(interpreter.Globals);
        for (var i = 0; i < declaration.Parameters.Count; i++)
        {
            environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
        }

        interpreter.ExecuteBlock(declaration.Body, environment);
        return null;
    }

    public override string ToString() => $"<fn {declaration.Name.Lexeme}>";
}
