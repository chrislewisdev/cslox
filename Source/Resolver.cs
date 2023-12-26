namespace CsLox;

public class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    private readonly Interpreter interpreter = new();
    private readonly Stack<Dictionary<string, bool>> scopes = new();

    public Resolver(Interpreter interpreter)
    {
        this.interpreter = interpreter;
    }

    public object VisitAssign(Expr.Assign expr)
    {
        throw new NotImplementedException();
    }

    public object VisitBinary(Expr.Binary expr)
    {
        throw new NotImplementedException();
    }

    public object VisitBlock(Stmt.Block stmt)
    {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return null;
    }

    public object VisitCall(Expr.Call expr)
    {
        throw new NotImplementedException();
    }

    public object VisitExpression(Stmt.Expression stmt)
    {
        throw new NotImplementedException();
    }

    public object VisitFunction(Stmt.Function stmt)
    {
        throw new NotImplementedException();
    }

    public object VisitGrouping(Expr.Grouping expr)
    {
        throw new NotImplementedException();
    }

    public object VisitIfCheck(Stmt.IfCheck stmt)
    {
        throw new NotImplementedException();
    }

    public object VisitLiteral(Expr.Literal expr)
    {
        throw new NotImplementedException();
    }

    public object VisitLogical(Expr.Logical expr)
    {
        throw new NotImplementedException();
    }

    public object VisitPrint(Stmt.Print stmt)
    {
        throw new NotImplementedException();
    }

    public object VisitReturn(Stmt.Return stmt)
    {
        throw new NotImplementedException();
    }

    public object VisitUnary(Expr.Unary expr)
    {
        throw new NotImplementedException();
    }

    public object VisitVariable(Expr.Variable expr)
    {
        throw new NotImplementedException();
    }

    public object VisitVariable(Stmt.Variable stmt)
    {
        Declare(stmt.Name);
        if (stmt.Initialiser != null)
        {
            Resolve(stmt.Initialiser);
        }
        Define(stmt.Name);
        return null;
    }

    public object VisitWhileLoop(Stmt.WhileLoop stmt)
    {
        throw new NotImplementedException();
    }

    private void BeginScope()
    {
        scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope()
    {
        scopes.Pop();
    }

    private void Declare(Token name)
    {
        if (!scopes.Any()) return;

        var scope = scopes.Peek();
        scope.Add(name.Lexeme, false);
    }

    private void Define(Token name)
    {
        if (!scopes.Any()) return;

        scopes.Peek()[name.Lexeme] = true;
    }

    private void Resolve(List<Stmt> statements)
    {
        foreach (var statement in statements)
        {
            Resolve(statement);
        }
    }

    private void Resolve(Stmt statement)
    {
        statement.AcceptVisitor(this);
    }

    private void Resolve(Expr expr)
    {
        expr.AcceptVisitor(this);
    }
}

