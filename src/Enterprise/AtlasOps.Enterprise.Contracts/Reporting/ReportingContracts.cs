namespace AtlasOps.Enterprise.Contracts.Reporting;

public enum ReportAggregation
{
    Count,
    Sum,
    Average,
    Minimum,
    Maximum,
}

public enum ReportSortDirection
{
    Ascending,
    Descending,
}

public enum DashboardWidgetKind
{
    Metric,
    Table,
    BarChart,
    Timeline,
}

public sealed record ReportColumn(
    string Name,
    string Expression,
    string DisplayName);

public sealed record ReportMeasure(
    string Name,
    string? Expression,
    ReportAggregation Aggregation,
    string DisplayName);

public sealed record ReportSort(
    string Field,
    ReportSortDirection Direction);

public sealed record ReportDefinition(
    string Id,
    string DisplayName,
    string? FilterExpression,
    IReadOnlyList<string> GroupByExpressions,
    IReadOnlyList<ReportColumn> Columns,
    IReadOnlyList<ReportMeasure> Measures,
    IReadOnlyList<ReportSort> Sorts);

public sealed record ReportRow(
    string Id,
    IReadOnlyDictionary<string, object?> Values);

public sealed record ReportDiagnostic(
    string Code,
    string Message,
    int? Position = null);

public sealed record ReportResultRow(
    IReadOnlyDictionary<string, object?> Dimensions,
    IReadOnlyDictionary<string, object?> Values);

public sealed record ReportResult(
    ReportDefinition Definition,
    IReadOnlyList<ReportResultRow> Rows,
    IReadOnlyList<ReportDiagnostic> Diagnostics,
    int InputRowCount,
    int FilteredRowCount,
    TimeSpan EvaluationDuration);

public sealed record DashboardWidgetDefinition(
    string Id,
    string DisplayName,
    DashboardWidgetKind Kind,
    string ReportId,
    string? ValueField,
    int MaximumRows = 10);

public sealed record DashboardWidgetResult(
    DashboardWidgetDefinition Definition,
    object? PrimaryValue,
    IReadOnlyList<ReportResultRow> Rows,
    IReadOnlyList<ReportDiagnostic> Diagnostics);
