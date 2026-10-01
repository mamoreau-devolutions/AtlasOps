namespace AtlasOps.Features.Analytics.MetricRetention;

public sealed record MetricRetentionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);