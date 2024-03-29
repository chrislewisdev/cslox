# For some reason, this requires Python 3.12 to run

def define_ast(name, ast):
    classes = [f"""
    public class {definition[0]} : {name}
    {{
{'\n'.join([f'\t\tpublic {field} {{ get; private set; }}' for field in definition[1]])}

        public {definition[0]}({', '.join(definition[1])})
        {{
{'\n'.join([f'\t\t\tthis.{field.split()[1]} = {field.split()[1]};' for field in definition[1]])}
        }}

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {{
            return v.Visit{definition[0]}(this);
        }}
    }}
""" for definition in ast]

    output = f"""namespace CsLox;

public abstract class {name}
{{
    public interface IVisitor<T>
    {{
{'\n'.join([f'\t\tT Visit{definition[0]}({definition[0]} {name.lower()});' for definition in ast])}
    }}

    public abstract T AcceptVisitor<T>(IVisitor<T> v);

    {''.join(classes)}
}}
"""

    with open(f"Source/{name}.cs", "w") as outputFile:
        outputFile.write(output)
    return

define_ast("Expr", [
    ('Assign', ['Token Name', 'Expr NewValue']),
    ('Binary', ['Expr Left', 'Token Operator', 'Expr Right']),
    ('Call', ['Expr Callee', 'Token Paren', 'List<Expr> Arguments']),
    ('Get', ['Expr Subject', 'Token Name']),
    ('Grouping', ['Expr Expression']),
    ('Literal', ['object Value']),
    ('Logical', ['Expr Left', 'Token Operator', 'Expr Right']),
    ('Set', ['Expr Subject', 'Token Name', 'Expr Value']),
    ('This', ['Token Keyword']),
    ('Super', ['Token Keyword', 'Token Method']),
    ('Unary', ['Token Operator', 'Expr Right']),
    ('Variable', ['Token Name']),
])

define_ast("Stmt", [
    ('Block', ['List<Stmt> Statements']),
    ('Class', ['Token Name', 'Expr.Variable Superclass', 'List<Stmt.Function> Methods']),
    ('Expression', ['Expr Subject']),
    ('Function', ['Token Name', 'List<Token> Parameters', 'List<Stmt> Body']),
    ('IfCheck', ['Expr Condition', 'Stmt ThenBranch', 'Stmt ElseBranch']),
    ('Print', ['Expr Subject']),
    ('Return', ['Token Keyword', 'Expr Subject']),
    ('Variable', ['Token Name', 'Expr Initialiser']),
    ('WhileLoop', ['Expr Condition', 'Stmt Body']),
])
