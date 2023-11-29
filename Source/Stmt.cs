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
		public Expr expression { get; private set; }

        public Expression(Expr expression)
        {
			this.expression = expression;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitExpression(this);
        }
    }

    public class Print : Stmt
    {
		public Expr expression { get; private set; }

        public Print(Expr expression)
        {
			this.expression = expression;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitPrint(this);
        }
    }

}
