namespace AtlasOps.Features.Analytics.ReportTemplate;

public sealed record ReportTemplateChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);