using System.Globalization;

namespace Lox;

using static TokenType;

public class Scanner(string source)
{
    private readonly List<Token> tokens = new();
    private int start = 0;
    private int current = 0;
    private int line = 1;

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(Eof, "", null, line));
        return tokens;
    }

    private void ScanToken()
    {
        switch (Advance())
        {
            case '(': AddToken(LeftParen); break;
            case ')': AddToken(RightParen); break;
            case '{': AddToken(LeftBrace); break;
            case '}': AddToken(RightBrace); break;
            case ',': AddToken(Comma); break;
            case '.': AddToken(Dot); break;
            case '-': AddToken(Minus); break;
            case '+': AddToken(Plus); break;
            case ';': AddToken(Semicolon); break;
            case '*': AddToken(Star); break;
            case '!':
                AddToken(Match('=') ? BangEqual : Bang);
                break;
            case '=':
                AddToken(Match('=') ? EqualEqual : Equal);
                break;
            case '<':
                AddToken(Match('=') ? LessEqual : Less);
                break;
            case '>':
                AddToken(Match('=') ? GreaterEqual : Greater);
                break;
            case '/' when Match('/'):
                while (Peek() != '\n' && !IsAtEnd())
                {
                    Advance();
                }

                break;
            case '/' when Match('*'): HandleBlockComment(); break;
            case '/':
                AddToken(Slash);

                break;
            case '"': HandleString(); break;
            case ' ' or '\r' or '\t': break;
            case '\n': line++; break;
            case var c when IsDigit(c): HandleNumber(); break;
            case var c when IsAlpha(c): HandleIdentifierOrKeyword(); break;
            case var c:
                Lox.Error(line, $"Unexpected character {c}.");
                break;
        }
    }

    private void HandleString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                line++;
            }

            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(line, "Unterminated string");
            return;
        }

        Advance(); // consume the closing quote

        AddToken(String, source[(start + 1)..(current - 1)]);
    }

    private void HandleNumber()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        AddToken(Number, double.Parse(source[start..current], CultureInfo.InvariantCulture));
    }

    private void HandleIdentifierOrKeyword()
    {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }

        var type = source[start..current] switch
        {
            "and" => And,
            "class" => Class,
            "else" => Else,
            "false" => False,
            "fun" => Fun,
            "for" => For,
            "if" => If,
            "nil" => Nil,
            "or" => Or,
            "print" => Print,
            "return" => Return,
            "super" => Super,
            "this" => This,
            "true" => True,
            "var" => Var,
            "while" => While,
            _ => Identifier,
        };

        AddToken(type);
    }

    private void HandleBlockComment()
    {
        while (!IsAtEnd())
        {
            switch (Advance())
            {
                case '\n':
                    line++;
                    break;
                case '*' when Match('/'):
                    return;
            }
        }

        Lox.Error(line, "Unterminated block comment");
    }

    private static bool IsDigit(char c) => c is >= '0' and <= '9';
    private static bool IsAlpha(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_';
    private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }

        if (source[current] != expected)
        {
            return false;
        }

        current++;
        return true;
    }

    private char Peek() => IsAtEnd() ? '\0' : source[current];

    private char PeekNext() => current + 1 >= source.Length
        ? '\0'
        : source[current + 1];

    private char Advance() => source[current++];

    private void AddToken(TokenType type, object? literal = null)
    {
        tokens.Add(new Token(type, source[start..current], literal, line));
    }

    private bool IsAtEnd() => current >= source.Length;
}
