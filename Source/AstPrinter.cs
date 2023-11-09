namespace CsLox;

public class AstPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr)
        => expr.AcceptVisitor(this);

    public string VisitBinary(Expr.Binary binary)
        => Parenthesize(binary.Operator.Lexeme, binary.Left, binary.Right);

    public string VisitGrouping(Expr.Grouping grouping)
        => Parenthesize("group", grouping.Expression);

    public string VisitLiteral(Expr.Literal literal) 
        => literal.Value == null ? "nil" : (literal.Value.ToString() ?? string.Empty);

    public string VisitTernary(Expr.Ternary ternary)
        => Parenthesize("?", ternary.Expression, ternary.Primary, ternary.Secondary);

    public string VisitUnary(Expr.Unary unary)
        => Parenthesize(unary.Operator.Lexeme, unary.Right);

    private string Parenthesize(string name, params Expr[] exprs)
        => $"({name} {string.Join(' ', exprs.Select(e => e.AcceptVisitor(this)))})";

}