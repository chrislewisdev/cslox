namespace CsLox;

public abstract class Expr
{
    public interface IVisitor<T>
    {
		T VisitAssign(Assign assign);
		T VisitBinary(Binary binary);
		T VisitGrouping(Grouping grouping);
		T VisitLiteral(Literal literal);
		T VisitLogical(Logical logical);
		T VisitUnary(Unary unary);
		T VisitVariable(Variable variable);
    }

    public abstract T AcceptVisitor<T>(IVisitor<T> v);

    
    public class Assign : Expr
    {
		public Token Name { get; private set; }
		public Expr NewValue { get; private set; }

        public Assign(Token Name, Expr NewValue)
        {
			this.Name = Name;
			this.NewValue = NewValue;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitAssign(this);
        }
    }

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

    public class Logical : Expr
    {
		public Expr Left { get; private set; }
		public Token Operator { get; private set; }
		public Expr Right { get; private set; }

        public Logical(Expr Left, Token Operator, Expr Right)
        {
			this.Left = Left;
			this.Operator = Operator;
			this.Right = Right;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitLogical(this);
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

    public class Variable : Expr
    {
		public Token Name { get; private set; }

        public Variable(Token Name)
        {
			this.Name = Name;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitVariable(this);
        }
    }

}
