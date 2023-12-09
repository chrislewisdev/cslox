namespace CsLox;

public abstract class Stmt
{
    public interface IVisitor<T>
    {
		T VisitExpression(Expression expression);
		T VisitPrint(Print print);
		T VisitVariable(Variable variable);
    }

    public abstract T AcceptVisitor<T>(IVisitor<T> v);

    
    public class Expression : Stmt
    {
		public Expr Subject { get; private set; }

        public Expression(Expr Subject)
        {
			this.Subject = Subject;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitExpression(this);
        }
    }

    public class Print : Stmt
    {
		public Expr Subject { get; private set; }

        public Print(Expr Subject)
        {
			this.Subject = Subject;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitPrint(this);
        }
    }

    public class Variable : Stmt
    {
		public Token Name { get; private set; }
		public Expr Initialiser { get; private set; }

        public Variable(Token Name, Expr Initialiser)
        {
			this.Name = Name;
			this.Initialiser = Initialiser;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitVariable(this);
        }
    }

}
