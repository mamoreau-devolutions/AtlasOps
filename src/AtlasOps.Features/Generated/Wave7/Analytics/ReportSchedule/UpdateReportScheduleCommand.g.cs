namespace AtlasOps.Features.Analytics.ReportSchedule;

public sealed record UpdateReportScheduleCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);