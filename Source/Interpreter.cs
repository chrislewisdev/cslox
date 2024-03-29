namespace CsLox;

public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    private class ClockCallable : ICallable
    {
        public int Arity => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
            => DateTimeOffset.Now.ToUnixTimeSeconds();

        public override string ToString() => "<native fn>";
    }

    public readonly Environment Globals = new();
    private readonly Dictionary<Expr, int> locals = new();
    private Environment environment;

    public Interpreter()
    {
        environment = Globals;
        Globals.Define("clock", new ClockCallable());
    }

    public void Resolve(Expr expr, int depth)
    {
        locals[expr] = depth;
    }

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var stmt in statements)
            {
                Execute(stmt);
            }
        }
        catch (RuntimeError e)
        {
            Lox.RuntimeError(e);
        }
    }

    public object VisitExpression(Stmt.Expression expression)
    {
        Evaluate(expression.Subject);
        return null;
    }

    public object VisitFunction(Stmt.Function function)
    {
        var loxFunction = new LoxFunction(function, environment, false);
        environment.Define(function.Name.Lexeme, loxFunction);
        return null;
    }

    public object VisitIfCheck(Stmt.IfCheck ifCheck)
    {
        if (IsTruthy(Evaluate(ifCheck.Condition))) {
            Execute(ifCheck.ThenBranch);
        } else if (ifCheck.ElseBranch != null) {
            Execute(ifCheck.ElseBranch);
        }
        return null;
    }

    public object VisitPrint(Stmt.Print print)
    {
        var result = Evaluate(print.Subject);
        Console.WriteLine(Stringify(result));
        return null;
    }

    public object VisitReturn(Stmt.Return stmt)
    {
        object @value = null;
        if (stmt.Subject != null) @value = Evaluate(stmt.Subject);

        throw new Return(@value);
    }

    public object VisitWhileLoop(Stmt.WhileLoop whileLoop)
    {
        while (IsTruthy(Evaluate(whileLoop.Condition)))
        {
            Execute(whileLoop.Body);
        }
        return null;
    }

    public object VisitBlock(Stmt.Block block)
    {
        ExecuteBlock(block.Statements, new Environment(environment));
        return null;
    }

    public object VisitClass(Stmt.Class stmt)
    {
        object superclass = null;
        if (stmt.Superclass != null)
        {
            superclass = Evaluate(stmt.Superclass);
            if (superclass is not LoxClass)
            {
                throw new RuntimeError(stmt.Superclass.Name, "Superclass must be a class.");
            }
        }

        environment.Define(stmt.Name.Lexeme, null);

        if (stmt.Superclass != null)
        {
            environment = new Environment(environment);
            environment.Define("super", superclass);
        }

        var methods = new Dictionary<string, LoxFunction>();
        foreach (var method in stmt.Methods)
        {
            var function = new LoxFunction(method, environment, method.Name.Lexeme == "init");
            methods[method.Name.Lexeme] = function;
        }

        var klass = new LoxClass(stmt.Name.Lexeme, superclass as LoxClass, methods);

        if (superclass != null)
        {
            environment = environment.Enclosing;
        }

        environment.Assign(stmt.Name, klass);
        return null;
    }

    public object VisitVariable(Stmt.Variable variable)
    {
        object @value = null;
        if (variable.Initialiser != null)
        {
            @value = Evaluate(variable.Initialiser);
        }

        environment.Define(variable.Name.Lexeme, @value);
        return null;
    }

    public object VisitAssign(Expr.Assign assign)
    {
        object @value = Evaluate(assign.NewValue);

        int? distance = locals.ContainsKey(assign) ? locals[assign] : null;
        if (distance != null)
        {
            environment.AssignAt(distance.Value, assign.Name, @value);
        }
        else
        {
            Globals.Assign(assign.Name, @value);
        }
        
        return @value;
    }

    public object VisitLiteral(Expr.Literal literal) => literal.Value;

    public object VisitLogical(Expr.Logical logical)
    {
        var left = Evaluate(logical.Left);

        if (logical.Operator.Type == TokenType.OR)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!IsTruthy(left)) return left;
        }

        return Evaluate(logical.Right);
    }

    public object VisitSet(Expr.Set expr)
    {
        var subject = Evaluate(expr.Subject);

        if (subject is not LoxInstance instance)
        {
            throw new RuntimeError(expr.Name, "Only instances have fields.");
        }

        var @value = Evaluate(expr.Value);
        instance.Set(expr.Name, @value);
        return @value;
    }

    public object VisitSuper(Expr.Super expr)
    {
        var distance = locals[expr];
        var superclass = environment.GetAt(distance, "super") as LoxClass;
        var instance = environment.GetAt(distance - 1, "this") as LoxInstance;
        var method = superclass.FindMethod(expr.Method.Lexeme);

        if (method == null) throw new RuntimeError(expr.Method, $"Undefined property {expr.Method.Lexeme}");

        return method.Bind(instance);
    }

    public object VisitThis(Expr.This expr)
    {
        return LookUpVariable(expr.Keyword, expr);
    }

    public object VisitVariable(Expr.Variable variable) => LookUpVariable(variable.Name, variable);

    public object VisitGrouping(Expr.Grouping grouping) => Evaluate(grouping.Expression);

    public object VisitUnary(Expr.Unary unary)
    {
        object right = Evaluate(unary.Right);

        switch (unary.Operator.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(unary.Operator, right);
                return -(double)right;
            default:
                return null;
        }
    }

    public object VisitBinary(Expr.Binary binary)
    {
        object left = Evaluate(binary.Left);
        object right = Evaluate(binary.Right);

        switch (binary.Operator.Type)
        {
            case TokenType.PLUS:
                if (left is double && right is double)
                {
                    return (double)left + (double)right;
                }
                else if (left is string && right is string)
                {
                    return (string)left + (string)right;
                }
                throw new RuntimeError(binary.Operator, "Operands must be two numbers or two strings.");
            case TokenType.MINUS:
                CheckNumberOperands(binary.Operator, left, right);
                return (double)left - (double)right;
            case TokenType.STAR:
                CheckNumberOperands(binary.Operator, left, right);
                return (double)left * (double)right;
            case TokenType.SLASH:
                CheckNumberOperands(binary.Operator, left, right);
                return (double)left / (double)right;
            case TokenType.EQUAL_EQUAL:
                return left?.Equals(right);
            case TokenType.BANG_EQUAL:
                return !left?.Equals(right);
            case TokenType.GREATER:
                CheckNumberOperands(binary.Operator, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(binary.Operator, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                CheckNumberOperands(binary.Operator, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(binary.Operator, left, right);
                return (double)left <= (double)right;
            default:
                return null;
        }
    }

    public object VisitCall(Expr.Call call)
    {
        var callee = Evaluate(call.Callee);
        var arguments = call.Arguments.Select(x => Evaluate(x)).ToList();
        var function = callee as ICallable
            ?? throw new RuntimeError(call.Paren, "Can only call functions and classes.");

        if (arguments.Count != function.Arity)
        {
            throw new RuntimeError(call.Paren, $"Expected {function.Arity} arguments but got {arguments.Count}.");
        }

        return function.Call(this, arguments);
    }

    public object VisitGet(Expr.Get expr)
    {
        var subject = Evaluate(expr.Subject);
        if (subject is LoxInstance instance)
        {
            return instance.Get(expr.Name);
        }

        throw new RuntimeError(expr.Name, "Only instances have properties.");
    }

    private void Execute(Stmt stmt) => stmt.AcceptVisitor(this);

    private object Evaluate(Expr expr) => expr.AcceptVisitor(this);

    public void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        var previousEnvironment = this.environment;
        try
        {
            this.environment = environment;
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            this.environment = previousEnvironment;
        }
    }

    private bool IsTruthy(object @value) => @value switch
    {
        null => false,
        false => false,
        _ => true,
    };

    private void CheckNumberOperand(Token @operator, object operand)
    {
        if (operand is double) return;
        throw new RuntimeError(@operator, "Operand must be a number.");
    }

    private void CheckNumberOperands(Token @operator, object left, object right)
    {
        if (left is double && right is double) return;
        throw new RuntimeError(@operator, "Operand must be a number.");
    }

    private string Stringify(object @value)
    {
        if (@value == null) return "nil";

        return @value.ToString() ?? string.Empty;
    }

    private object LookUpVariable(Token name, Expr expr)
    {
        int? distance = locals.ContainsKey(expr) ? locals[expr] : null;

        if (distance != null)
        {
            return environment.GetAt(distance.Value, name.Lexeme);
        }
        else
        {
            return Globals.Get(name);
        }
    }
}
