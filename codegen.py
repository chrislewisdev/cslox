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
{'\n'.join([f'\t\tT Visit{definition[0]}({definition[0]} {definition[0].lower()});' for definition in ast])}
    }}

    public abstract T AcceptVisitor<T>(IVisitor<T> v);

    {''.join(classes)}
}}
"""

    with open(f"Source/{name}.cs", "w") as outputFile:
        outputFile.write(output)
    return

define_ast("Expr", [
    ('Binary', ['Expr Left', 'Token Operator', 'Expr Right']),
    ('Grouping', ['Expr Expression']),
    ('Literal', ['object? Value']),
    ('Unary', ['Token Operator', 'Expr Right']),
])

define_ast("Stmt", [
    ('Expression', ['Expr expression']),
    ('Print', ['Expr expression']),
])
