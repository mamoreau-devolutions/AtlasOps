namespace AtlasOps.Features.Analytics.ReportDefinition;

public sealed record UpdateReportDefinitionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);