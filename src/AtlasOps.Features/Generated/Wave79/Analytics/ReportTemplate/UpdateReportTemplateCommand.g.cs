namespace AtlasOps.Features.Analytics.ReportTemplate;

public sealed record UpdateReportTemplateCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);