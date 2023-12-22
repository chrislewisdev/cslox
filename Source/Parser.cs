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
            if (Match(TokenType.VAR)) return VarDeclaration();

            return Statement();
        }
        catch (ParseError)
        {
            Synchronise();
            return null;
        }
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
        if (Match(TokenType.IF)) return IfCheckStatement();
        if (Match(TokenType.PRINT)) return PrintStatement();
        if (Match(TokenType.WHILE)) return WhileStatement();
        if (Match(TokenType.LEFT_BRACE)) return Block();
        return ExpressionStatement();
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

    private Stmt WhileStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after if.");
        var condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

        var body = Statement();

        return new Stmt.WhileLoop(condition, body);
    }

    private Stmt Block()
    {
        var statements = new List<Stmt>();

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd)
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
        return new Stmt.Block(statements);
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
            return Primary();
        }
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
