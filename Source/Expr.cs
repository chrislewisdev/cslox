namespace CsLox;

public abstract class Expr
{
    public interface IVisitor<T>
    {
		T VisitAssign(Assign expr);
		T VisitBinary(Binary expr);
		T VisitCall(Call expr);
		T VisitGet(Get expr);
		T VisitGrouping(Grouping expr);
		T VisitLiteral(Literal expr);
		T VisitLogical(Logical expr);
		T VisitSet(Set expr);
		T VisitThis(This expr);
		T VisitSuper(Super expr);
		T VisitUnary(Unary expr);
		T VisitVariable(Variable expr);
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

    public class Call : Expr
    {
		public Expr Callee { get; private set; }
		public Token Paren { get; private set; }
		public List<Expr> Arguments { get; private set; }

        public Call(Expr Callee, Token Paren, List<Expr> Arguments)
        {
			this.Callee = Callee;
			this.Paren = Paren;
			this.Arguments = Arguments;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitCall(this);
        }
    }

    public class Get : Expr
    {
		public Expr Subject { get; private set; }
		public Token Name { get; private set; }

        public Get(Expr Subject, Token Name)
        {
			this.Subject = Subject;
			this.Name = Name;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitGet(this);
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

    public class Set : Expr
    {
		public Expr Subject { get; private set; }
		public Token Name { get; private set; }
		public Expr Value { get; private set; }

        public Set(Expr Subject, Token Name, Expr Value)
        {
			this.Subject = Subject;
			this.Name = Name;
			this.Value = Value;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitSet(this);
        }
    }

    public class This : Expr
    {
		public Token Keyword { get; private set; }

        public This(Token Keyword)
        {
			this.Keyword = Keyword;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitThis(this);
        }
    }

    public class Super : Expr
    {
		public Token Keyword { get; private set; }
		public Token Method { get; private set; }

        public Super(Token Keyword, Token Method)
        {
			this.Keyword = Keyword;
			this.Method = Method;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitSuper(this);
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
