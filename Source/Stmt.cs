namespace CsLox;

public abstract class Stmt
{
    public interface IVisitor<T>
    {
		T VisitExpression(Expression expression);
		T VisitPrint(Print print);
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

}
