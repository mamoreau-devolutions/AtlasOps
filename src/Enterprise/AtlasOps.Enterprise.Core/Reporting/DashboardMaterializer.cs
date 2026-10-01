namespace AtlasOps.Enterprise.Core.Reporting;

using AtlasOps.Enterprise.Contracts.Reporting;

public sealed class DashboardMaterializer
{
    public DashboardWidgetResult Materialize(DashboardWidgetDefinition widget, ReportResult report)
    {
        if (!string.Equals(widget.ReportId, report.Definition.Id, StringComparison.OrdinalIgnoreCase))
        {
            return new(
                widget,
                null,
                [],
                [new("dashboard.report.mismatch", $"Widget '{widget.DisplayName}' expects report '{widget.ReportId}'.")]);
        }

        ReportResultRow[] rows = report.Rows.Take(Math.Max(0, widget.MaximumRows)).ToArray();
        object? primaryValue = null;
        if (!string.IsNullOrWhiteSpace(widget.ValueField) && rows.Length > 0)
        {
            if (!rows[0].Values.TryGetValue(widget.ValueField, out primaryValue))
            {
                rows[0].Dimensions.TryGetValue(widget.ValueField, out primaryValue);
            }
        }

        List<ReportDiagnostic> diagnostics = [.. report.Diagnostics];
        if (!string.IsNullOrWhiteSpace(widget.ValueField) && primaryValue is null)
        {
            diagnostics.Add(new("dashboard.value.missing", $"Value field '{widget.ValueField}' was not found."));
        }

        return new(widget, primaryValue, rows, diagnostics);
    }
}
