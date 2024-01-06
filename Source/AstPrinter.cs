namespace CsLox;

public class AstPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr)
        => expr.AcceptVisitor(this);

    public string VisitAssign(Expr.Assign assign)
        => Parenthesize(assign.Name.Lexeme, assign.NewValue);

    public string VisitBinary(Expr.Binary binary)
        => Parenthesize(binary.Operator.Lexeme, binary.Left, binary.Right);

    public string VisitCall(Expr.Call call)
        => Parenthesize("call", new List<Expr>{ call.Callee }.Union(call.Arguments).ToArray());

    public string VisitGet(Expr.Get get)
        => Parenthesize($"get {get.Name}", get.Subject);

    public string VisitGrouping(Expr.Grouping grouping)
        => Parenthesize("group", grouping.Expression);

    public string VisitLiteral(Expr.Literal literal) 
        => literal.Value == null ? "nil" : (literal.Value.ToString() ?? string.Empty);

    public string VisitLogical(Expr.Logical logical)
        => Parenthesize(logical.Operator.Lexeme, logical.Left, logical.Right);

    public string VisitUnary(Expr.Unary unary)
        => Parenthesize(unary.Operator.Lexeme, unary.Right);

    public string VisitVariable(Expr.Variable variable)
        => variable.Name.Lexeme;

    private string Parenthesize(string name, params Expr[] exprs)
        => $"({name} {string.Join(' ', exprs.Select(e => e.AcceptVisitor(this)))})";

}
