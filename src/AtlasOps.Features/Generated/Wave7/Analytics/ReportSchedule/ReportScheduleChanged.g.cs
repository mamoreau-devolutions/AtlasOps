namespace AtlasOps.Features.Analytics.ReportSchedule;

public sealed record ReportScheduleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);