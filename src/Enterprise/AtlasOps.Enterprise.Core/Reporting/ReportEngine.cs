namespace AtlasOps.Enterprise.Core.Reporting;

using System.Diagnostics;
using System.Globalization;

using AtlasOps.Enterprise.Contracts.Reporting;

public sealed class ReportEngine
{
    private readonly ReportExpressionCompiler compiler = new();

    public ReportResult Execute(ReportDefinition definition, IReadOnlyList<ReportRow> input)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        List<ReportDiagnostic> diagnostics = [];
        List<ReportRow> filtered = [];

        foreach (ReportRow row in input)
        {
            if (string.IsNullOrWhiteSpace(definition.FilterExpression))
            {
                filtered.Add(row);
                continue;
            }

            bool valid = this.compiler.TryEvaluateBoolean(definition.FilterExpression, row, out bool included, out IReadOnlyList<ReportDiagnostic> filterDiagnostics);
            diagnostics.AddRange(filterDiagnostics.Select(diagnostic => diagnostic with { Message = $"Row '{row.Id}': {diagnostic.Message}" }));
            if (valid && included)
            {
                filtered.Add(row);
            }
        }

        IReadOnlyList<ReportResultRow> results = definition.GroupByExpressions.Count == 0
            ? this.ProjectRows(definition, filtered, diagnostics)
            : this.AggregateRows(definition, filtered, diagnostics);
        results = this.Sort(results, definition.Sorts);
        stopwatch.Stop();
        return new(definition, results, diagnostics, input.Count, filtered.Count, stopwatch.Elapsed);
    }

    private IReadOnlyList<ReportResultRow> ProjectRows(
        ReportDefinition definition,
        IReadOnlyList<ReportRow> rows,
        ICollection<ReportDiagnostic> diagnostics)
    {
        List<ReportResultRow> results = [];
        foreach (ReportRow row in rows)
        {
            Dictionary<string, object?> values = new(StringComparer.OrdinalIgnoreCase);
            foreach (ReportColumn column in definition.Columns)
            {
                ReportExpressionResult result = this.compiler.Evaluate(column.Expression, row);
                AddDiagnostics(
                    diagnostics,
                    result.Diagnostics,
                    $"Column '{column.DisplayName}', row '{row.Id}': ");
                values[column.Name] = result.Value;
            }

            foreach (ReportMeasure measure in definition.Measures)
            {
                values[measure.Name] = measure.Aggregation == ReportAggregation.Count
                    ? 1
                    : EvaluateMeasureValue(measure, row, this.compiler, diagnostics);
            }

            results.Add(new(new Dictionary<string, object?> { ["rowId"] = row.Id }, values));
        }

        return results;
    }

    private IReadOnlyList<ReportResultRow> AggregateRows(
        ReportDefinition definition,
        IReadOnlyList<ReportRow> rows,
        ICollection<ReportDiagnostic> diagnostics)
    {
        Dictionary<GroupKey, List<ReportRow>> groups = new();
        foreach (ReportRow row in rows)
        {
            List<object?> keys = [];
            foreach (string groupExpression in definition.GroupByExpressions)
            {
                ReportExpressionResult result = this.compiler.Evaluate(groupExpression, row);
                AddDiagnostics(
                    diagnostics,
                    result.Diagnostics,
                    $"Group '{groupExpression}', row '{row.Id}': ");
                keys.Add(result.Value);
            }

            GroupKey key = new(keys);
            if (!groups.TryGetValue(key, out List<ReportRow>? groupRows))
            {
                groupRows = [];
                groups[key] = groupRows;
            }

            groupRows.Add(row);
        }

        List<ReportResultRow> results = [];
        foreach (KeyValuePair<GroupKey, List<ReportRow>> group in groups.OrderBy(static pair => pair.Key.SortKey, StringComparer.OrdinalIgnoreCase))
        {
            Dictionary<string, object?> dimensions = new(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < definition.GroupByExpressions.Count; index++)
            {
                dimensions[definition.GroupByExpressions[index]] = group.Key.Values[index];
            }

            Dictionary<string, object?> values = new(StringComparer.OrdinalIgnoreCase);
            foreach (ReportMeasure measure in definition.Measures)
            {
                values[measure.Name] = this.AggregateMeasure(measure, group.Value, diagnostics);
            }

            results.Add(new(dimensions, values));
        }

        return results;
    }

    private object? AggregateMeasure(
        ReportMeasure measure,
        IReadOnlyList<ReportRow> rows,
        ICollection<ReportDiagnostic> diagnostics)
    {
        if (measure.Aggregation == ReportAggregation.Count)
        {
            return rows.Count;
        }

        List<decimal> values = [];
        foreach (ReportRow row in rows)
        {
            object? rawValue = EvaluateMeasureValue(measure, row, this.compiler, diagnostics);
            if (decimal.TryParse(Convert.ToString(rawValue, CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number))
            {
                values.Add(number);
            }
            else
            {
                diagnostics.Add(new("report.measure.numeric", $"Measure '{measure.DisplayName}' requires numeric values; row '{row.Id}' was ignored."));
            }
        }

        if (values.Count == 0)
        {
            return null;
        }

        return measure.Aggregation switch
        {
            ReportAggregation.Sum => values.Sum(),
            ReportAggregation.Average => values.Average(),
            ReportAggregation.Minimum => values.Min(),
            ReportAggregation.Maximum => values.Max(),
            _ => null,
        };
    }

    private IReadOnlyList<ReportResultRow> Sort(
        IReadOnlyList<ReportResultRow> rows,
        IReadOnlyList<ReportSort> sorts)
    {
        IOrderedEnumerable<ReportResultRow>? ordered = null;
        foreach (ReportSort sort in sorts)
        {
            Func<ReportResultRow, string> selector = row => Convert.ToString(GetValue(row, sort.Field), CultureInfo.InvariantCulture) ?? string.Empty;
            ordered = ordered is null
                ? sort.Direction == ReportSortDirection.Ascending
                    ? rows.OrderBy(selector, StringComparer.OrdinalIgnoreCase)
                    : rows.OrderByDescending(selector, StringComparer.OrdinalIgnoreCase)
                : sort.Direction == ReportSortDirection.Ascending
                    ? ordered.ThenBy(selector, StringComparer.OrdinalIgnoreCase)
                    : ordered.ThenByDescending(selector, StringComparer.OrdinalIgnoreCase);
        }

        return ordered?.ToArray() ?? rows;
    }

    private static object? EvaluateMeasureValue(
        ReportMeasure measure,
        ReportRow row,
        ReportExpressionCompiler compiler,
        ICollection<ReportDiagnostic> diagnostics)
    {
        if (string.IsNullOrWhiteSpace(measure.Expression))
        {
            return null;
        }

        ReportExpressionResult result = compiler.Evaluate(measure.Expression, row);
        AddDiagnostics(
            diagnostics,
            result.Diagnostics,
            $"Measure '{measure.DisplayName}', row '{row.Id}': ");
        return result.Value;
    }

    private static void AddDiagnostics(
        ICollection<ReportDiagnostic> destination,
        IReadOnlyList<ReportDiagnostic> source,
        string prefix)
    {
        foreach (ReportDiagnostic diagnostic in source)
        {
            destination.Add(diagnostic with { Message = prefix + diagnostic.Message });
        }
    }

    private static object? GetValue(ReportResultRow row, string field)
    {
        if (row.Values.TryGetValue(field, out object? value))
        {
            return value;
        }

        return row.Dimensions.TryGetValue(field, out value) ? value : null;
    }

    private sealed class GroupKey : IEquatable<GroupKey>
    {
        public GroupKey(IReadOnlyList<object?> values)
        {
            this.Values = values;
            this.SortKey = string.Join("\u001F", values.Select(value => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "<null>"));
        }

        public IReadOnlyList<object?> Values { get; }

        public string SortKey { get; }

        public bool Equals(GroupKey? other)
        {
            return other is not null && string.Equals(this.SortKey, other.SortKey, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? value)
        {
            return value is GroupKey other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(this.SortKey);
        }
    }
}
