namespace AtlasOps.Features.Analytics.ReportDefinition;

public sealed record ReportDefinitionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);