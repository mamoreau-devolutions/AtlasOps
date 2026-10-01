namespace AtlasOps.Enterprise.Core.Reporting;

using System.Globalization;

using AtlasOps.Enterprise.Contracts.Reporting;

public sealed class ReportExpressionCompiler
{
    public ReportExpressionResult Evaluate(string expression, ReportRow row)
    {
        ReportExpressionLexer lexer = new(expression);
        IReadOnlyList<ReportToken> tokens = lexer.Tokenize();
        ReportExpressionParser parser = new(tokens);
        ReportExpressionNode? syntax = parser.Parse();
        List<ReportDiagnostic> diagnostics = [.. lexer.Diagnostics, .. parser.Diagnostics];
        if (syntax is null || diagnostics.Count > 0)
        {
            return new(false, null, diagnostics);
        }

        object? value = this.EvaluateNode(syntax, row, diagnostics);
        return new(diagnostics.Count == 0, value, diagnostics);
    }

    public bool TryEvaluateBoolean(string expression, ReportRow row, out bool value, out IReadOnlyList<ReportDiagnostic> diagnostics)
    {
        ReportExpressionResult result = this.Evaluate(expression, row);
        diagnostics = result.Diagnostics;
        value = ToBoolean(result.Value);
        return result.IsValid;
    }

    private object? EvaluateNode(ReportExpressionNode node, ReportRow row, ICollection<ReportDiagnostic> diagnostics)
    {
        switch (node)
        {
            case ReportLiteralNode literal:
                return literal.Value;
            case ReportIdentifierNode identifier:
                if (row.Values.TryGetValue(identifier.Name, out object? value))
                {
                    return value;
                }

                diagnostics.Add(new("expression.field.missing", $"Field '{identifier.Name}' does not exist.", identifier.Position));
                return null;
            case ReportUnaryNode unary:
                return this.EvaluateUnary(unary, row, diagnostics);
            case ReportBinaryNode binary:
                return this.EvaluateBinary(binary, row, diagnostics);
            default:
                diagnostics.Add(new("expression.node.unknown", "Expression contains an unknown syntax node.", node.Position));
                return null;
        }
    }

    private object? EvaluateUnary(ReportUnaryNode unary, ReportRow row, ICollection<ReportDiagnostic> diagnostics)
    {
        object? operand = this.EvaluateNode(unary.Operand, row, diagnostics);
        if (unary.Operator == ReportTokenKind.Not)
        {
            return !ToBoolean(operand);
        }

        if (unary.Operator == ReportTokenKind.Minus && TryDecimal(operand, out decimal number))
        {
            return -number;
        }

        diagnostics.Add(new("expression.unary.invalid", $"Unary operator '{unary.Operator}' cannot be applied.", unary.Position));
        return null;
    }

    private object? EvaluateBinary(ReportBinaryNode binary, ReportRow row, ICollection<ReportDiagnostic> diagnostics)
    {
        object? left = this.EvaluateNode(binary.Left, row, diagnostics);
        if (binary.Operator == ReportTokenKind.And && !ToBoolean(left))
        {
            return false;
        }

        if (binary.Operator == ReportTokenKind.Or && ToBoolean(left))
        {
            return true;
        }

        object? right = this.EvaluateNode(binary.Right, row, diagnostics);
        return binary.Operator switch
        {
            ReportTokenKind.And => ToBoolean(left) && ToBoolean(right),
            ReportTokenKind.Or => ToBoolean(left) || ToBoolean(right),
            ReportTokenKind.Equal => ValuesEqual(left, right),
            ReportTokenKind.NotEqual => !ValuesEqual(left, right),
            ReportTokenKind.Greater => Compare(left, right, static value => value > 0),
            ReportTokenKind.GreaterOrEqual => Compare(left, right, static value => value >= 0),
            ReportTokenKind.Less => Compare(left, right, static value => value < 0),
            ReportTokenKind.LessOrEqual => Compare(left, right, static value => value <= 0),
            ReportTokenKind.Plus => Add(left, right),
            ReportTokenKind.Minus => Numeric(left, right, static (a, b) => a - b, binary.Position, diagnostics),
            ReportTokenKind.Star => Numeric(left, right, static (a, b) => a * b, binary.Position, diagnostics),
            ReportTokenKind.Slash => Divide(left, right, binary.Position, diagnostics),
            _ => null,
        };
    }

    private static object? Add(object? left, object? right)
    {
        if (TryDecimal(left, out decimal leftNumber) && TryDecimal(right, out decimal rightNumber))
        {
            return leftNumber + rightNumber;
        }

        return string.Concat(Convert.ToString(left, CultureInfo.InvariantCulture), Convert.ToString(right, CultureInfo.InvariantCulture));
    }

    private static object? Numeric(
        object? left,
        object? right,
        Func<decimal, decimal, decimal> operation,
        int position,
        ICollection<ReportDiagnostic> diagnostics)
    {
        if (TryDecimal(left, out decimal leftNumber) && TryDecimal(right, out decimal rightNumber))
        {
            return operation(leftNumber, rightNumber);
        }

        diagnostics.Add(new("expression.number.required", "Arithmetic operators require numeric operands.", position));
        return null;
    }

    private static object? Divide(object? left, object? right, int position, ICollection<ReportDiagnostic> diagnostics)
    {
        if (!TryDecimal(left, out decimal leftNumber) || !TryDecimal(right, out decimal rightNumber))
        {
            diagnostics.Add(new("expression.number.required", "Division requires numeric operands.", position));
            return null;
        }

        if (rightNumber == 0m)
        {
            diagnostics.Add(new("expression.divide.zero", "Division by zero is not allowed.", position));
            return null;
        }

        return leftNumber / rightNumber;
    }

    private static bool Compare(object? left, object? right, Func<int, bool> predicate)
    {
        if (TryDecimal(left, out decimal leftNumber) && TryDecimal(right, out decimal rightNumber))
        {
            return predicate(leftNumber.CompareTo(rightNumber));
        }

        string leftText = Convert.ToString(left, CultureInfo.InvariantCulture) ?? string.Empty;
        string rightText = Convert.ToString(right, CultureInfo.InvariantCulture) ?? string.Empty;
        return predicate(string.Compare(leftText, rightText, StringComparison.OrdinalIgnoreCase));
    }

    private static bool ValuesEqual(object? left, object? right)
    {
        if (left is null || right is null)
        {
            return left is null && right is null;
        }

        if (TryDecimal(left, out decimal leftNumber) && TryDecimal(right, out decimal rightNumber))
        {
            return leftNumber == rightNumber;
        }

        return string.Equals(
            Convert.ToString(left, CultureInfo.InvariantCulture),
            Convert.ToString(right, CultureInfo.InvariantCulture),
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool ToBoolean(object? value)
    {
        return value switch
        {
            bool boolean => boolean,
            null => false,
            string text => !string.IsNullOrWhiteSpace(text) && !string.Equals(text, "false", StringComparison.OrdinalIgnoreCase),
            _ when TryDecimal(value, out decimal number) => number != 0m,
            _ => true,
        };
    }

    private static bool TryDecimal(object? value, out decimal number)
    {
        return decimal.TryParse(
            Convert.ToString(value, CultureInfo.InvariantCulture),
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out number);
    }
}
