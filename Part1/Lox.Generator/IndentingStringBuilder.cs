using System;
using System.Text;

namespace Lox.Generator;

public sealed class IndentingStringBuilder
{
    private const char IndentChar = ' ';
    private const int IndentSize = 4;

    private readonly StringBuilder builder = new();
    private int level = 0;
    private bool startOfLine = true;

    public IndentingStringBuilder Append(string s)
    {
        if (startOfLine)
        {
            builder.Append(IndentChar, level * IndentSize);
            startOfLine = false;
        }

        builder.Append(s);
        return this;
    }

    public IndentingStringBuilder AppendLine()
    {
        builder.AppendLine();
        startOfLine = true;
        return this;
    }

    public IndentingStringBuilder AppendLine(string s)
    {
        if (startOfLine)
        {
            builder.Append(IndentChar, level * IndentSize);
        }

        builder.AppendLine(s);
        startOfLine = true;
        return this;
    }

    public IndentingStringBuilderBlock StartBlock(string s)
    {
        if (startOfLine)
        {
            builder.Append(IndentChar, level * IndentSize);
        }

        builder.AppendLine(s);
        builder.Append(IndentChar, level * IndentSize).AppendLine("{");
        level++;
        startOfLine = true;

        return new IndentingStringBuilderBlock(this);
    }

    private void EndBlock()
    {
        if (level < 1)
        {
            throw new InvalidOperationException($"{nameof(EndBlock)} was called more times than {nameof(StartBlock)}.");
        }

        if (!startOfLine)
        {
            builder.AppendLine();
        }

        level--;
        builder.Append(IndentChar, level * IndentSize).AppendLine("}");
        startOfLine = true;
    }

    public override string ToString() => builder.ToString();

    public struct IndentingStringBuilderBlock(IndentingStringBuilder sb) : IDisposable
    {
        public void Dispose() => sb.EndBlock();
    }
}
