namespace CsLox;

public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    private Environment environment = new();

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

    public object VisitBlock(Stmt.Block block)
    {
        ExecuteBlock(block.Statements, new Environment(environment));
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
        environment.Assign(assign.Name, @value);
        return @value;
    }

    public object VisitLiteral(Expr.Literal literal) => literal.Value;

    public object VisitVariable(Expr.Variable variable) => environment.Get(variable.Name);

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

    private void Execute(Stmt stmt) => stmt.AcceptVisitor(this);

    private object Evaluate(Expr expr) => expr.AcceptVisitor(this);

    private void ExecuteBlock(List<Stmt> statements, Environment environment)
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
}
