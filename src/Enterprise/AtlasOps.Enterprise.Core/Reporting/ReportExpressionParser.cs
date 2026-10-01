namespace AtlasOps.Enterprise.Core.Reporting;

using AtlasOps.Enterprise.Contracts.Reporting;

internal sealed class ReportExpressionParser
{
    private readonly IReadOnlyList<ReportToken> tokens;
    private readonly List<ReportDiagnostic> diagnostics = [];
    private int position;

    public ReportExpressionParser(IReadOnlyList<ReportToken> tokens)
    {
        this.tokens = tokens;
    }

    public IReadOnlyList<ReportDiagnostic> Diagnostics => this.diagnostics;

    public ReportExpressionNode? Parse()
    {
        ReportExpressionNode? expression = this.ParseOr();
        if (this.Current.Kind != ReportTokenKind.End)
        {
            this.diagnostics.Add(new("expression.token.unexpected", $"Unexpected token '{this.Current.Text}'.", this.Current.Position));
        }

        return expression;
    }

    private ReportExpressionNode? ParseOr()
    {
        ReportExpressionNode? left = this.ParseAnd();
        while (this.Current.Kind == ReportTokenKind.Or)
        {
            ReportToken operation = this.Consume();
            ReportExpressionNode? right = this.ParseAnd();
            if (left is null || right is null)
            {
                return left;
            }

            left = new ReportBinaryNode(left, operation.Kind, right, operation.Position);
        }

        return left;
    }

    private ReportExpressionNode? ParseAnd()
    {
        ReportExpressionNode? left = this.ParseEquality();
        while (this.Current.Kind == ReportTokenKind.And)
        {
            ReportToken operation = this.Consume();
            ReportExpressionNode? right = this.ParseEquality();
            if (left is null || right is null)
            {
                return left;
            }

            left = new ReportBinaryNode(left, operation.Kind, right, operation.Position);
        }

        return left;
    }

    private ReportExpressionNode? ParseEquality()
    {
        ReportExpressionNode? left = this.ParseComparison();
        while (this.Current.Kind is ReportTokenKind.Equal or ReportTokenKind.NotEqual)
        {
            ReportToken operation = this.Consume();
            ReportExpressionNode? right = this.ParseComparison();
            if (left is null || right is null)
            {
                return left;
            }

            left = new ReportBinaryNode(left, operation.Kind, right, operation.Position);
        }

        return left;
    }

    private ReportExpressionNode? ParseComparison()
    {
        ReportExpressionNode? left = this.ParseTerm();
        while (this.Current.Kind is ReportTokenKind.Greater or ReportTokenKind.GreaterOrEqual or ReportTokenKind.Less or ReportTokenKind.LessOrEqual)
        {
            ReportToken operation = this.Consume();
            ReportExpressionNode? right = this.ParseTerm();
            if (left is null || right is null)
            {
                return left;
            }

            left = new ReportBinaryNode(left, operation.Kind, right, operation.Position);
        }

        return left;
    }

    private ReportExpressionNode? ParseTerm()
    {
        ReportExpressionNode? left = this.ParseFactor();
        while (this.Current.Kind is ReportTokenKind.Plus or ReportTokenKind.Minus)
        {
            ReportToken operation = this.Consume();
            ReportExpressionNode? right = this.ParseFactor();
            if (left is null || right is null)
            {
                return left;
            }

            left = new ReportBinaryNode(left, operation.Kind, right, operation.Position);
        }

        return left;
    }

    private ReportExpressionNode? ParseFactor()
    {
        ReportExpressionNode? left = this.ParseUnary();
        while (this.Current.Kind is ReportTokenKind.Star or ReportTokenKind.Slash)
        {
            ReportToken operation = this.Consume();
            ReportExpressionNode? right = this.ParseUnary();
            if (left is null || right is null)
            {
                return left;
            }

            left = new ReportBinaryNode(left, operation.Kind, right, operation.Position);
        }

        return left;
    }

    private ReportExpressionNode? ParseUnary()
    {
        if (this.Current.Kind is ReportTokenKind.Not or ReportTokenKind.Minus)
        {
            ReportToken operation = this.Consume();
            ReportExpressionNode? operand = this.ParseUnary();
            return operand is null ? null : new ReportUnaryNode(operation.Kind, operand, operation.Position);
        }

        return this.ParsePrimary();
    }

    private ReportExpressionNode? ParsePrimary()
    {
        ReportToken token = this.Current;
        switch (token.Kind)
        {
            case ReportTokenKind.String:
            case ReportTokenKind.Number:
            case ReportTokenKind.True:
            case ReportTokenKind.False:
            case ReportTokenKind.Null:
                this.Consume();
                return new ReportLiteralNode(token.Value, token.Position);
            case ReportTokenKind.Identifier:
                this.Consume();
                return new ReportIdentifierNode(token.Text, token.Position);
            case ReportTokenKind.OpenParenthesis:
                this.Consume();
                ReportExpressionNode? inner = this.ParseOr();
                if (this.Current.Kind != ReportTokenKind.CloseParenthesis)
                {
                    this.diagnostics.Add(new("expression.parenthesis.missing", "A closing parenthesis is required.", this.Current.Position));
                }
                else
                {
                    this.Consume();
                }

                return inner;
            default:
                this.diagnostics.Add(new("expression.value.expected", $"Expected a value but found '{token.Text}'.", token.Position));
                this.Consume();
                return null;
        }
    }

    private ReportToken Current => this.position < this.tokens.Count ? this.tokens[this.position] : this.tokens[^1];

    private ReportToken Consume()
    {
        ReportToken current = this.Current;
        this.position++;
        return current;
    }
}
