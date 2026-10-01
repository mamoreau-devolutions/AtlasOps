namespace AtlasOps.Enterprise.Core.Reporting;

using AtlasOps.Enterprise.Contracts.Reporting;

internal enum ReportTokenKind
{
    End,
    Identifier,
    String,
    Number,
    True,
    False,
    Null,
    OpenParenthesis,
    CloseParenthesis,
    Plus,
    Minus,
    Star,
    Slash,
    Equal,
    NotEqual,
    Greater,
    GreaterOrEqual,
    Less,
    LessOrEqual,
    And,
    Or,
    Not,
}

internal sealed record ReportToken(ReportTokenKind Kind, string Text, int Position, object? Value = null);

internal abstract record ReportExpressionNode(int Position);

internal sealed record ReportLiteralNode(object? Value, int LiteralPosition) : ReportExpressionNode(LiteralPosition);

internal sealed record ReportIdentifierNode(string Name, int IdentifierPosition) : ReportExpressionNode(IdentifierPosition);

internal sealed record ReportUnaryNode(
    ReportTokenKind Operator,
    ReportExpressionNode Operand,
    int OperatorPosition) : ReportExpressionNode(OperatorPosition);

internal sealed record ReportBinaryNode(
    ReportExpressionNode Left,
    ReportTokenKind Operator,
    ReportExpressionNode Right,
    int OperatorPosition) : ReportExpressionNode(OperatorPosition);

public sealed record ReportExpressionResult(
    bool IsValid,
    object? Value,
    IReadOnlyList<ReportDiagnostic> Diagnostics);
