namespace CsLox;

public class Parser
{
    private class ParseError : Exception {}

    private readonly List<Token> tokens;
    private int current = 0;

    private bool IsAtEnd => Peek().Type == TokenType.EOF;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        var statements = new List<Stmt>();
        while (!IsAtEnd)
        {
            statements.Add(Declaration());
        }
        return statements;
    }

    private Stmt Declaration()
    {
        try
        {
            if (Match(TokenType.CLASS)) return ClassDeclaration();
            if (Match(TokenType.FUN)) return Function("function");
            if (Match(TokenType.VAR)) return VarDeclaration();

            return Statement();
        }
        catch (ParseError)
        {
            Synchronise();
            return null;
        }
    }

    private Stmt ClassDeclaration()
    {
        var name = Consume(TokenType.IDENTIFIER, "Expect class name.");
        Consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");

        var methods = new List<Stmt.Function>();
        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd)
        {
            methods.Add(Function("method"));
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");
        return new Stmt.Class(name, methods);
    }

    private Stmt.Function Function(string kind)
    {
        var name = Consume(TokenType.IDENTIFIER, $"Expect {kind} name.");
        Consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");

        var parameters = new List<Token>();
        if (!Check(TokenType.RIGHT_PAREN))
        {
            if (parameters.Count >= 255) Error(Peek(), "Can't have more than 255 parameters.");

            do
            {
                parameters.Add(Consume(TokenType.IDENTIFIER, "Expect parameter name."));
            } while(Match(TokenType.COMMA));
        }
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

        Consume(TokenType.LEFT_BRACE, $"Expect '{{' before {kind} body.");
        var body = Block();
        return new Stmt.Function(name, parameters, body);
    }

    private Stmt VarDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

        Expr initializer = null;
        if (Match(TokenType.EQUAL))
        {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
        return new Stmt.Variable(name, initializer);
    }

    private Stmt Statement()
    {
        if (Match(TokenType.FOR)) return ForStatement();
        if (Match(TokenType.IF)) return IfCheckStatement();
        if (Match(TokenType.PRINT)) return PrintStatement();
        if (Match(TokenType.RETURN)) return ReturnStatement();
        if (Match(TokenType.WHILE)) return WhileStatement();
        if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());
        return ExpressionStatement();
    }

    private Stmt ForStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after for.");
        
        Stmt initialiser = null;
        if (!Match(TokenType.SEMICOLON))
        {
            if (Match(TokenType.VAR))
            {
                initialiser = VarDeclaration();
            }
            else
            {
                initialiser = ExpressionStatement();
            }
        }

        Expr condition = null;
        if (!Check(TokenType.SEMICOLON))
        {
            condition = Expression();
        }
        Consume(TokenType.SEMICOLON, "Expect ';' after for condition.");

        Expr increment = null;
        if (!Check(TokenType.RIGHT_PAREN))
        {
            increment = Expression();
        }

        Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

        var body = Statement();

        if (increment != null)
        {
            body = new Stmt.Block(new List<Stmt> { body, new Stmt.Expression(increment) });
        }

        condition ??= new Expr.Literal(true);
        body = new Stmt.WhileLoop(condition, body);

        if (initialiser != null)
        {
            body = new Stmt.Block(new List<Stmt> { initialiser, body });
        }

        return body;
    }

    private Stmt IfCheckStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after if.");
        var condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

        var thenBranch = Statement();
        var elseBranch = Match(TokenType.ELSE) ? Statement() : null;

        return new Stmt.IfCheck(condition, thenBranch, elseBranch);
    }

    private Stmt PrintStatement()
    {
        Expr value = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new Stmt.Print(value);
    }

    private Stmt ReturnStatement()
    {
        var keyword = Previous();
        var subject = !Check(TokenType.SEMICOLON) ? Expression() : null;

        Consume(TokenType.SEMICOLON, "Expect ';' after return.");

        return new Stmt.Return(keyword, subject);
    }

    private Stmt WhileStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after while.");
        var condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after while condition.");

        var body = Statement();

        return new Stmt.WhileLoop(condition, body);
    }

    private List<Stmt> Block()
    {
        var statements = new List<Stmt>();

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd)
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
        return statements;
    }

    private Stmt ExpressionStatement()
    {
        Expr value = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new Stmt.Expression(value);
    }

    private Expr Expression() => Assignment();

    private Expr Assignment()
    {
        var expr = Or();

        if (Match(TokenType.EQUAL))
        {
            var equals = Previous();
            var newValue = Assignment();

            if (expr is Expr.Variable variable)
            {
                Token name = variable.Name;
                return new Expr.Assign(name, newValue);
            }
            else if (expr is Expr.Get get)
            {
                return new Expr.Set(get.Subject, get.Name, newValue);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Expr Or() {
        var expr = And();

        if (Match(TokenType.OR))
        {
            var @operator = Previous();
            var right = And();
            return new Expr.Logical(expr, @operator, right);
        }

        return expr;
    }

    private Expr And() {
        var expr = Equality();

        if (Match(TokenType.AND))
        {
            var @operator = Previous();
            var right = Equality();
            return new Expr.Logical(expr, @operator, right);
        }

        return expr;
    }

    private Expr Equality()
    {
        var expr = Comparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            var @operator = Previous();
            var right = Comparison();
            expr = new Expr.Binary(expr, @operator, right);
        }

        return expr;
    }

    private Expr Comparison()
    {
        var expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            var @operator = Previous();
            var right = Term();
            expr = new Expr.Binary(expr, @operator, right);
        }

        return expr;
    }

    private Expr Term()
    {
       var expr = Factor();

       while (Match(TokenType.PLUS, TokenType.MINUS))
       {
           var @operator = Previous();
           var right = Factor();
           expr = new Expr.Binary(expr, @operator, right);
       }

       return expr;
    }

    private Expr Factor()
    {
        var expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            var @operator = Previous();
            var right = Unary();
            expr = new Expr.Binary(expr, @operator, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            return new Expr.Unary(Previous(), Unary());
        }
        else
        {
            return Call();
        }
    }

    private Expr Call()
    {
        var expr = Primary();

        while (true)
        {
            if (Match(TokenType.LEFT_PAREN))
            {
                expr = FinishCall(expr);
            }
            else if (Match(TokenType.DOT))
            {
                var name = Consume(TokenType.IDENTIFIER, "Expect identifier after '.'.");
                expr = new Expr.Get(expr, name);
            }
            else
            {
                break;
            }
        }

        return expr;
    }

    private Expr FinishCall(Expr callee)
    {
        var arguments = new List<Expr>();

        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 arguments.");
                }
                arguments.Add(Expression());
            } while (Match(TokenType.COMMA));
        }

        var paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");

        return new Expr.Call(callee, paren, arguments);
    }

    private Expr Primary()
    {
        if (Match(TokenType.TRUE))  return new Expr.Literal(true);
        if (Match(TokenType.FALSE)) return new Expr.Literal(false);
        if (Match(TokenType.NIL))   return new Expr.Literal(null);

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Expr.Literal(Previous().Literal);
        }

        if (Match(TokenType.THIS)) return new Expr.This(Previous());

        if (Match(TokenType.IDENTIFIER))
        {
            return new Expr.Variable(Previous());
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            var expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private void Synchronise()
    {
        Advance();

        while (!IsAtEnd)
        {
            if (Previous().Type == TokenType.SEMICOLON) return;

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd) current++;
        return Previous();
    }

    private Token Peek()
    {
        return tokens[current];
    }

    private Token Previous()
    {
        return tokens[current - 1];
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message)
    {
        Lox.Error(token, message);
        return new ParseError();
    }
}
