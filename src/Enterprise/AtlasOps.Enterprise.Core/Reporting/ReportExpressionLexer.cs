namespace AtlasOps.Enterprise.Core.Reporting;

using System.Globalization;

using AtlasOps.Enterprise.Contracts.Reporting;

internal sealed class ReportExpressionLexer
{
    private readonly string text;
    private readonly List<ReportDiagnostic> diagnostics = [];
    private int position;

    public ReportExpressionLexer(string text)
    {
        this.text = text;
    }

    public IReadOnlyList<ReportDiagnostic> Diagnostics => this.diagnostics;

    public IReadOnlyList<ReportToken> Tokenize()
    {
        List<ReportToken> tokens = [];
        ReportToken token;
        do
        {
            token = this.ReadToken();
            tokens.Add(token);
        }
        while (token.Kind != ReportTokenKind.End);

        return tokens;
    }

    private ReportToken ReadToken()
    {
        this.SkipWhitespace();
        if (this.position >= this.text.Length)
        {
            return new(ReportTokenKind.End, string.Empty, this.position);
        }

        int start = this.position;
        char current = this.text[this.position];
        if (char.IsLetter(current) || current is '_' or '$')
        {
            return this.ReadIdentifier();
        }

        if (char.IsDigit(current) || current == '.' && this.Peek(1) is char next && char.IsDigit(next))
        {
            return this.ReadNumber();
        }

        if (current is '"' or '\'')
        {
            return this.ReadString();
        }

        this.position++;
        return current switch
        {
            '(' => new(ReportTokenKind.OpenParenthesis, "(", start),
            ')' => new(ReportTokenKind.CloseParenthesis, ")", start),
            '+' => new(ReportTokenKind.Plus, "+", start),
            '-' => new(ReportTokenKind.Minus, "-", start),
            '*' => new(ReportTokenKind.Star, "*", start),
            '/' => new(ReportTokenKind.Slash, "/", start),
            '=' when this.Match('=') => new(ReportTokenKind.Equal, "==", start),
            '!' when this.Match('=') => new(ReportTokenKind.NotEqual, "!=", start),
            '!' => new(ReportTokenKind.Not, "!", start),
            '>' when this.Match('=') => new(ReportTokenKind.GreaterOrEqual, ">=", start),
            '>' => new(ReportTokenKind.Greater, ">", start),
            '<' when this.Match('=') => new(ReportTokenKind.LessOrEqual, "<=", start),
            '<' => new(ReportTokenKind.Less, "<", start),
            '&' when this.Match('&') => new(ReportTokenKind.And, "&&", start),
            '|' when this.Match('|') => new(ReportTokenKind.Or, "||", start),
            _ => this.CreateInvalidToken(current, start),
        };
    }

    private ReportToken ReadIdentifier()
    {
        int start = this.position;
        while (this.position < this.text.Length)
        {
            char current = this.text[this.position];
            if (!char.IsLetterOrDigit(current) && current is not '_' and not '.' and not '$')
            {
                break;
            }

            this.position++;
        }

        string value = this.text[start..this.position];
        return value.ToLowerInvariant() switch
        {
            "true" => new(ReportTokenKind.True, value, start, true),
            "false" => new(ReportTokenKind.False, value, start, false),
            "null" => new(ReportTokenKind.Null, value, start),
            "and" => new(ReportTokenKind.And, value, start),
            "or" => new(ReportTokenKind.Or, value, start),
            "not" => new(ReportTokenKind.Not, value, start),
            _ => new(ReportTokenKind.Identifier, value, start, value),
        };
    }

    private ReportToken ReadNumber()
    {
        int start = this.position;
        bool hasDecimalPoint = false;
        while (this.position < this.text.Length)
        {
            char current = this.text[this.position];
            if (current == '.' && !hasDecimalPoint)
            {
                hasDecimalPoint = true;
                this.position++;
                continue;
            }

            if (!char.IsDigit(current))
            {
                break;
            }

            this.position++;
        }

        string value = this.text[start..this.position];
        if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number))
        {
            this.diagnostics.Add(new("expression.number.invalid", $"'{value}' is not a valid number.", start));
            return new(ReportTokenKind.Number, value, start);
        }

        return new(ReportTokenKind.Number, value, start, number);
    }

    private ReportToken ReadString()
    {
        int start = this.position;
        char quote = this.text[this.position++];
        System.Text.StringBuilder builder = new();
        bool terminated = false;

        while (this.position < this.text.Length)
        {
            char current = this.text[this.position++];
            if (current == quote)
            {
                terminated = true;
                break;
            }

            if (current == '\\' && this.position < this.text.Length)
            {
                char escaped = this.text[this.position++];
                builder.Append(escaped switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '\\' => '\\',
                    '"' => '"',
                    '\'' => '\'',
                    _ => escaped,
                });
            }
            else
            {
                builder.Append(current);
            }
        }

        if (!terminated)
        {
            this.diagnostics.Add(new("expression.string.unterminated", "String literal is not terminated.", start));
        }

        return new(ReportTokenKind.String, this.text[start..this.position], start, builder.ToString());
    }

    private ReportToken CreateInvalidToken(char current, int start)
    {
        this.diagnostics.Add(new("expression.character.invalid", $"Character '{current}' is not valid in an expression.", start));
        return new(ReportTokenKind.Identifier, current.ToString(), start, current.ToString());
    }

    private void SkipWhitespace()
    {
        while (this.position < this.text.Length && char.IsWhiteSpace(this.text[this.position]))
        {
            this.position++;
        }
    }

    private char? Peek(int offset)
    {
        int index = this.position + offset;
        return index < this.text.Length ? this.text[index] : null;
    }

    private bool Match(char expected)
    {
        if (this.position >= this.text.Length || this.text[this.position] != expected)
        {
            return false;
        }

        this.position++;
        return true;
    }
}
