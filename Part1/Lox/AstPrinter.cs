using System.Text;

namespace Lox;

public class AstPrinter : Expr.Visitor<string>
{
    public string Print(Expr expr) => expr.Accept(this);

    public string VisitBinary(Expr.Binary expr) => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

    public string VisitGrouping(Expr.Grouping expr) => Parenthesize("group", expr.Expression);

    public string VisitLiteral(Expr.Literal expr) => expr.Value?.ToString() ?? "nil";

    public string VisitUnary(Expr.Unary expr) => Parenthesize(expr.Operator.Lexeme, expr.Right);

    private string Parenthesize(string name, params ReadOnlySpan<Expr> exprs)
    {
        var sb = new StringBuilder();
        sb.Append($"({name}");
        foreach (var expr in exprs)
        {
            sb.Append($" {expr.Accept(this)}");
        }

        sb.Append(')');

        return sb.ToString();
    }
}
