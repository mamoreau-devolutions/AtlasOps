namespace AtlasOps.Enterprise.Core.Workflow;

using System.Globalization;

using AtlasOps.Enterprise.Contracts.Workflow;

public sealed class WorkflowConditionEvaluator
{
    private static readonly string[] Operators = [">=", "<=", "!=", "==", ">", "<"];

    public WorkflowConditionResult Evaluate(string? expression, WorkflowConditionContext context)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return new(true, true, "No condition was specified.");
        }

        string condition = expression.Trim();
        if (condition.StartsWith("exists(", StringComparison.OrdinalIgnoreCase) && condition.EndsWith(')'))
        {
            string key = condition[7..^1].Trim();
            bool exists = context.Variables.TryGetValue(key, out object? value) && value is not null;
            return new(true, exists, exists ? $"Variable '{key}' exists." : $"Variable '{key}' does not exist.");
        }

        string? selectedOperator = Operators.FirstOrDefault(condition.Contains);
        if (selectedOperator is null)
        {
            return new(false, false, $"Condition '{condition}' does not contain a supported operator.");
        }

        string[] parts = condition.Split(selectedOperator, 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
        {
            return new(false, false, $"Condition '{condition}' is malformed.");
        }

        string variableName = parts[0];
        if (!context.Variables.TryGetValue(variableName, out object? actual))
        {
            return new(true, false, $"Variable '{variableName}' is not defined.");
        }

        object? expected = ParseLiteral(parts[1]);
        bool result = Compare(actual, expected, selectedOperator);
        return new(true, result, $"Compared '{variableName}' using '{selectedOperator}' and evaluated to {result}.");
    }

    private static object? ParseLiteral(string text)
    {
        string value = text.Trim().Trim('"', '\'');
        if (string.Equals(value, "null", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (bool.TryParse(value, out bool boolean))
        {
            return boolean;
        }

        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number))
        {
            return number;
        }

        return value;
    }

    private static bool Compare(object? actual, object? expected, string operation)
    {
        if (operation is "==" or "!=")
        {
            bool equals = ValuesEqual(actual, expected);
            return operation == "==" ? equals : !equals;
        }

        if (!TryDecimal(actual, out decimal actualNumber) || !TryDecimal(expected, out decimal expectedNumber))
        {
            return false;
        }

        return operation switch
        {
            ">" => actualNumber > expectedNumber,
            ">=" => actualNumber >= expectedNumber,
            "<" => actualNumber < expectedNumber,
            "<=" => actualNumber <= expectedNumber,
            _ => false,
        };
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

        return string.Equals(Convert.ToString(left, CultureInfo.InvariantCulture), Convert.ToString(right, CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryDecimal(object? value, out decimal number)
    {
        return decimal.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out number);
    }
}
