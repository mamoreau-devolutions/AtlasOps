namespace AtlasOps.Features.Analytics.MetricDefinition;

public sealed record MetricDefinitionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);