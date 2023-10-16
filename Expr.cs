namespace CsLox;

public abstract class Expr
{
    public interface IVisitor<T>
    {
		T AcceptBinary(Binary binary);
		T AcceptGrouping(Grouping grouping);
		T AcceptLiteral(Literal literal);
		T AcceptUnary(Unary unary);
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
            return v.AcceptBinary(this);
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
            return v.AcceptGrouping(this);
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
            return v.AcceptLiteral(this);
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
            return v.AcceptUnary(this);
        }
    }

}
