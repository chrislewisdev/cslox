ast = [
    ('Binary', ['Expr Left', 'Token Operator', 'Expr Right']),
    ('Grouping', ['Expr Expression']),
    ('Literal', ['object Value']),
    ('Unary', ['Token Operator', 'Expr Right']),
]

expressions = [f"""
    public class {expr[0]} : Expr
    {{
{'\n'.join([f'\t\tpublic {field} {{ get; private set; }}' for field in expr[1]])}

        public {expr[0]}({', '.join(expr[1])})
        {{
{'\n'.join([f'\t\t\tthis.{field.split()[1]} = {field.split()[1]};' for field in expr[1]])}
        }}

        public override T AcceptVisitor<T>(IVisitor<T> v)
        {{
            return v.Visit{expr[0]}(this);
        }}
    }}
""" for expr in ast]

output = f"""namespace CsLox;

public abstract class Expr
{{
    public interface IVisitor<T>
    {{
{'\n'.join([f'\t\tT Visit{expr[0]}({expr[0]} {expr[0].lower()});' for expr in ast])}
    }}

    public abstract T AcceptVisitor<T>(IVisitor<T> v);

    {''.join(expressions)}
}}
"""

with open("Source/Expr.cs", "w") as outputFile:
    outputFile.write(output)
