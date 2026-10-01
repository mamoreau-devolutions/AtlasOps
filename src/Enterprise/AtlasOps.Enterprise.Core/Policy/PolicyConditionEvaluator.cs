namespace AtlasOps.Enterprise.Core.Policy;

using System.Globalization;

using AtlasOps.Enterprise.Contracts.Policy;

public sealed class PolicyConditionEvaluator
{
    public bool Evaluate(PolicyCondition condition, PolicyRequest request, out string explanation)
    {
        IReadOnlyDictionary<string, string>? source = condition.Source.ToLowerInvariant() switch
        {
            "subject" => request.Subject.Attributes,
            "resource" => request.Resource.Attributes,
            "environment" => request.Environment,
            _ => null,
        };

        if (source is null)
        {
            explanation = $"Condition source '{condition.Source}' is unknown.";
            return false;
        }

        bool exists = source.TryGetValue(condition.Key, out string? actual);
        if (condition.Operator == PolicyConditionOperator.Exists)
        {
            explanation = exists ? $"'{condition.Source}.{condition.Key}' exists." : $"'{condition.Source}.{condition.Key}' does not exist.";
            return exists;
        }

        if (!exists)
        {
            explanation = $"'{condition.Source}.{condition.Key}' is missing.";
            return false;
        }

        actual ??= string.Empty;
        string expected = condition.ExpectedValue ?? string.Empty;
        bool result = condition.Operator switch
        {
            PolicyConditionOperator.Equals => string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
            PolicyConditionOperator.NotEquals => !string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
            PolicyConditionOperator.Contains => actual.Contains(expected, StringComparison.OrdinalIgnoreCase),
            PolicyConditionOperator.StartsWith => actual.StartsWith(expected, StringComparison.OrdinalIgnoreCase),
            PolicyConditionOperator.EndsWith => actual.EndsWith(expected, StringComparison.OrdinalIgnoreCase),
            PolicyConditionOperator.In => expected.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Contains(actual, StringComparer.OrdinalIgnoreCase),
            PolicyConditionOperator.GreaterThan => CompareNumbers(actual, expected, static comparison => comparison > 0),
            PolicyConditionOperator.GreaterThanOrEqual => CompareNumbers(actual, expected, static comparison => comparison >= 0),
            PolicyConditionOperator.LessThan => CompareNumbers(actual, expected, static comparison => comparison < 0),
            PolicyConditionOperator.LessThanOrEqual => CompareNumbers(actual, expected, static comparison => comparison <= 0),
            _ => false,
        };
        explanation = $"'{condition.Source}.{condition.Key}' {condition.Operator} '{expected}' evaluated to {result}.";
        return result;
    }

    private static bool CompareNumbers(string actual, string expected, Func<int, bool> predicate)
    {
        bool actualParsed = decimal.TryParse(actual, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal actualNumber);
        bool expectedParsed = decimal.TryParse(expected, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal expectedNumber);
        return actualParsed && expectedParsed && predicate(actualNumber.CompareTo(expectedNumber));
    }
}
