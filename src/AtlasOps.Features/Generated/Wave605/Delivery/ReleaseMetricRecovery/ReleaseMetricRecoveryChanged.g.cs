namespace AtlasOps.Features.Delivery.ReleaseMetricRecovery;

public sealed record ReleaseMetricRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);