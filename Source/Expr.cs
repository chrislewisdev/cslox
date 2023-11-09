namespace CsLox;

public abstract class Expr
{
    public interface IVisitor<T>
    {
		T VisitBinary(Binary binary);
		T VisitGrouping(Grouping grouping);
		T VisitLiteral(Literal literal);
		T VisitUnary(Unary unary);
		T VisitTernary(Ternary ternary);
    }

    public abstract T AcceptVisitor<T>(IVisitor<T> v);

    
    public class Binary : Expr
    {
		public Expr Left { get; private set; }
		public Token Operator { get; private set; }
		public Expr Right { get; private set; }

        public Binary(Expr Left, Token Operator, Expr Right)
        {
			this.Left = Left;
			this.Operator = Operator;
			this.Right = Right;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitBinary(this);
        }
    }

    public class Grouping : Expr
    {
		public Expr Expression { get; private set; }

        public Grouping(Expr Expression)
        {
			this.Expression = Expression;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitGrouping(this);
        }
    }

    public class Literal : Expr
    {
		public object Value { get; private set; }

        public Literal(object Value)
        {
			this.Value = Value;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitLiteral(this);
        }
    }

    public class Unary : Expr
    {
		public Token Operator { get; private set; }
		public Expr Right { get; private set; }

        public Unary(Token Operator, Expr Right)
        {
			this.Operator = Operator;
			this.Right = Right;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitUnary(this);
        }
    }

    public class Ternary : Expr
    {
		public Expr Expression { get; private set; }
		public Expr Primary { get; private set; }
		public Expr Secondary { get; private set; }

        public Ternary(Expr Expression, Expr Primary, Expr Secondary)
        {
			this.Expression = Expression;
			this.Primary = Primary;
			this.Secondary = Secondary;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitTernary(this);
        }
    }

}
