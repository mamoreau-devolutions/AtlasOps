namespace AtlasOps.Features.Analytics.MetricSample;

public sealed record MetricSampleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);