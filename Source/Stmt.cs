namespace CsLox;

public abstract class Stmt
{
    public interface IVisitor<T>
    {
		T VisitBlock(Block stmt);
		T VisitClass(Class stmt);
		T VisitExpression(Expression stmt);
		T VisitFunction(Function stmt);
		T VisitIfCheck(IfCheck stmt);
		T VisitPrint(Print stmt);
		T VisitReturn(Return stmt);
		T VisitVariable(Variable stmt);
		T VisitWhileLoop(WhileLoop stmt);
    }

    public abstract T AcceptVisitor<T>(IVisitor<T> v);

    
    public class Block : Stmt
    {
		public List<Stmt> Statements { get; private set; }

        public Block(List<Stmt> Statements)
        {
			this.Statements = Statements;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitBlock(this);
        }
    }

    public class Class : Stmt
    {
		public Token Name { get; private set; }
		public List<Stmt.Function> Methods { get; private set; }

        public Class(Token Name, List<Stmt.Function> Methods)
        {
			this.Name = Name;
			this.Methods = Methods;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitClass(this);
        }
    }

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

    public class Function : Stmt
    {
		public Token Name { get; private set; }
		public List<Token> Parameters { get; private set; }
		public List<Stmt> Body { get; private set; }

        public Function(Token Name, List<Token> Parameters, List<Stmt> Body)
        {
			this.Name = Name;
			this.Parameters = Parameters;
			this.Body = Body;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitFunction(this);
        }
    }

    public class IfCheck : Stmt
    {
		public Expr Condition { get; private set; }
		public Stmt ThenBranch { get; private set; }
		public Stmt ElseBranch { get; private set; }

        public IfCheck(Expr Condition, Stmt ThenBranch, Stmt ElseBranch)
        {
			this.Condition = Condition;
			this.ThenBranch = ThenBranch;
			this.ElseBranch = ElseBranch;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitIfCheck(this);
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

    public class Return : Stmt
    {
		public Token Keyword { get; private set; }
		public Expr Subject { get; private set; }

        public Return(Token Keyword, Expr Subject)
        {
			this.Keyword = Keyword;
			this.Subject = Subject;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitReturn(this);
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

    public class WhileLoop : Stmt
    {
		public Expr Condition { get; private set; }
		public Stmt Body { get; private set; }

        public WhileLoop(Expr Condition, Stmt Body)
        {
			this.Condition = Condition;
			this.Body = Body;
        }

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {
            return v.VisitWhileLoop(this);
        }
    }

}
