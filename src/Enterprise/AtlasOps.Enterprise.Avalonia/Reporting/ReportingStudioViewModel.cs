namespace AtlasOps.Enterprise.Avalonia.Reporting;

using System.Globalization;

using AtlasOps.Enterprise.Avalonia.Common;
using AtlasOps.Enterprise.Contracts.Reporting;
using AtlasOps.Enterprise.Core.Reporting;
using AtlasOps.Enterprise.Core.Scenarios;

using global::Avalonia.Collections;

public sealed class ReportingStudioViewModel : EnterpriseViewModelBase
{
    private readonly ReportEngine reportEngine = new();
    private readonly DashboardMaterializer dashboardMaterializer = new();
    private readonly IReadOnlyList<ReportRow> sourceRows;
    private readonly IReadOnlyList<ReportDefinition> allReports;
    private ReportDefinition? selectedReport;

    public ReportingStudioViewModel()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        this.sourceRows = EnterpriseScenarioCatalog.CreateOperationalRows(now);
        this.allReports = EnterpriseScenarioCatalog.CreateReports();
        this.RefreshReports();
        this.SelectedReport = this.Reports.FirstOrDefault();
    }

    public override string Title => "Query, reporting, and dashboard studio";

    public override string Summary => "Parse typed expressions, filter operational rows, aggregate measures, sort results, and materialize dashboard widgets.";

    public AvaloniaList<ReportDefinition> Reports { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> Results { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> Diagnostics { get; } = [];

    public AvaloniaList<EnterpriseMetric> Metrics { get; } = [];

    public ReportDefinition? SelectedReport
    {
        get => this.selectedReport;
        set
        {
            if (this.selectedReport == value)
            {
                return;
            }

            this.selectedReport = value;
            this.OnPropertyChanged();
            this.RefreshResult();
        }
    }

    public string FilterExpression => this.SelectedReport?.FilterExpression ?? "No filter";

    protected override void OnSearchChanged()
    {
        this.RefreshReports();
    }

    private void RefreshReports()
    {
        IEnumerable<ReportDefinition> filtered = this.allReports;
        if (!string.IsNullOrWhiteSpace(this.SearchText))
        {
            filtered = filtered.Where(
                report =>
                    report.DisplayName.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    (report.FilterExpression?.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        this.Reports.Clear();
        this.Reports.AddRange(filtered);
        if (this.SelectedReport is not null && !this.Reports.Contains(this.SelectedReport))
        {
            this.SelectedReport = this.Reports.FirstOrDefault();
        }
    }

    private void RefreshResult()
    {
        this.Results.Clear();
        this.Diagnostics.Clear();
        this.Metrics.Clear();
        this.OnPropertyChanged(nameof(this.FilterExpression));
        if (this.SelectedReport is null)
        {
            return;
        }

        ReportResult result = this.reportEngine.Execute(this.SelectedReport, this.sourceRows);
        this.Results.AddRange(
            result.Rows.Select(
                (row, index) => new EnterpriseDetailRow(
                    BuildTitle(row, index),
                    BuildSubtitle(row),
                    $"ROW {index + 1}")));
        this.Diagnostics.AddRange(
            result.Diagnostics.Select(
                diagnostic => new EnterpriseDetailRow(diagnostic.Code, diagnostic.Message, diagnostic.Position?.ToString(CultureInfo.InvariantCulture) ?? "REPORT")));
        DashboardWidgetDefinition widget = new(
            $"widget-{this.SelectedReport.Id}",
            this.SelectedReport.DisplayName,
            DashboardWidgetKind.Metric,
            this.SelectedReport.Id,
            this.SelectedReport.Measures.FirstOrDefault()?.Name,
            8);
        DashboardWidgetResult widgetResult = this.dashboardMaterializer.Materialize(widget, result);
        this.Metrics.AddRange(
        [
            new("Input rows", result.InputRowCount.ToString(), "Operational source records"),
            new("Filtered rows", result.FilteredRowCount.ToString(), this.FilterExpression),
            new("Result rows", result.Rows.Count.ToString(), $"{result.EvaluationDuration.TotalMilliseconds:F1} ms"),
            new("Widget value", Convert.ToString(widgetResult.PrimaryValue, CultureInfo.InvariantCulture) ?? "—", widget.Kind.ToString()),
        ]);
    }

    private static string BuildTitle(ReportResultRow row, int index)
    {
        if (row.Dimensions.Count > 0)
        {
            return string.Join(" · ", row.Dimensions.Select(static pair => $"{pair.Key}: {pair.Value}"));
        }

        if (row.Values.TryGetValue("id", out object? identifier))
        {
            return Convert.ToString(identifier, CultureInfo.InvariantCulture) ?? $"Result {index + 1}";
        }

        return $"Result {index + 1}";
    }

    private static string BuildSubtitle(ReportResultRow row)
    {
        return string.Join(
            " · ",
            row.Values.Select(
                static pair => $"{pair.Key}: {Convert.ToString(pair.Value, CultureInfo.InvariantCulture) ?? "—"}"));
    }
}
