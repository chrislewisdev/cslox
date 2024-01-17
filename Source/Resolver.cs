namespace CsLox;

public class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    private enum FunctionType
    {
        NONE,
        FUNCTION,
        METHOD,
    }

    private enum ClassType
    {
        NONE,
        CLASS,
    }

    private readonly Interpreter interpreter = new();
    private readonly Stack<Dictionary<string, bool>> scopes = new();
    private FunctionType currentFunction = FunctionType.NONE;
    private ClassType currentClass = ClassType.NONE;

    public Resolver(Interpreter interpreter)
    {
        this.interpreter = interpreter;
    }

    public object VisitAssign(Expr.Assign expr)
    {
        Resolve(expr.NewValue);
        ResolveLocal(expr, expr.Name);
        return null;
    }

    public object VisitBinary(Expr.Binary expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object VisitBlock(Stmt.Block stmt)
    {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return null;
    }

    public object VisitClass(Stmt.Class stmt)
    {
        var enclosingClass = currentClass;
        currentClass = ClassType.CLASS;

        Declare(stmt.Name);
        Define(stmt.Name);

        BeginScope();
        scopes.Peek().Add("this", true);

        foreach (var method in stmt.Methods)
        {
            var declaration = FunctionType.METHOD;
            ResolveFunction(method, declaration);
        }

        currentClass = enclosingClass;

        EndScope();;

        return null;
    }

    public object VisitCall(Expr.Call expr)
    {
        Resolve(expr.Callee);

        foreach (var argument in expr.Arguments)
        {
            Resolve(argument);
        }

        return null;
    }

    public object VisitGet(Expr.Get expr)
    {
        Resolve(expr.Subject);
        return null;
    }

    public object VisitExpression(Stmt.Expression stmt)
    {
        Resolve(stmt.Subject);
        return null;
    }

    public object VisitFunction(Stmt.Function stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);

        ResolveFunction(stmt, FunctionType.FUNCTION);
        return null;
    }

    public object VisitGrouping(Expr.Grouping expr)
    {
        Resolve(expr.Expression);
        return null;
    }

    public object VisitIfCheck(Stmt.IfCheck stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.ThenBranch);
        if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);
        return null;
    }

    public object VisitLiteral(Expr.Literal expr)
    {
        return null;
    }

    public object VisitLogical(Expr.Logical expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object VisitSet(Expr.Set expr)
    {
        Resolve(expr.Subject);
        Resolve(expr.Value);
        return null;
    }

    public object VisitThis(Expr.This expr)
    {
        if (currentClass == ClassType.NONE)
        {
            Lox.Error(expr.Keyword, "Can't use 'this' outside of a class.");
            return null;
        }

        ResolveLocal(expr, expr.Keyword);
        return null;
    }

    public object VisitPrint(Stmt.Print stmt)
    {
        Resolve(stmt.Subject);
        return null;
    }

    public object VisitReturn(Stmt.Return stmt)
    {
        if (currentFunction == FunctionType.NONE) Lox.Error(stmt.Keyword, "Can't return from top-level code.");

        if (stmt.Subject != null) Resolve(stmt.Subject);
        return null;
    }

    public object VisitUnary(Expr.Unary expr)
    {
        Resolve(expr.Right);
        return null;
    }

    public object VisitVariable(Expr.Variable expr)
    {
        if (scopes.Any() && scopes.Peek().ContainsKey(expr.Name.Lexeme) && scopes.Peek()[expr.Name.Lexeme] == false)
        {
            Lox.Error(expr.Name, "Can't read local variable in its own initialiser.");
        }

        ResolveLocal(expr, expr.Name);
        return null;
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
        Resolve(stmt.Condition);
        Resolve(stmt.Body);
        return null;
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
        if (scope.ContainsKey(name.Lexeme)) Lox.Error(name, "A variable with this name already exists in this scope.");
        scope.Add(name.Lexeme, false);
    }

    private void Define(Token name)
    {
        if (!scopes.Any()) return;

        scopes.Peek()[name.Lexeme] = true;
    }

    public void Resolve(List<Stmt> statements)
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

    private void ResolveLocal(Expr expr, Token name)
    {
        // Can't index into a stack, so copying it to a list
        // We could *probably* just enumerate the stack instead, but this is closer to the book's code
        var scopesArray = scopes.ToList();
        scopesArray.Reverse();

        for (var i = scopes.Count - 1; i >= 0; i--)
        {
            if (scopesArray[i].ContainsKey(name.Lexeme))
            {
                interpreter.Resolve(expr, scopes.Count - 1 - i);
                return;
            }
        }
    }

    private void ResolveFunction(Stmt.Function function, FunctionType type)
    {
        var enclosingFunction = currentFunction;
        currentFunction = type;

        BeginScope();
        foreach (var param in function.Parameters)
        {
            Declare(param);
            Define(param);
        }
        Resolve(function.Body);
        EndScope();

        currentFunction = enclosingFunction;
    }
}

