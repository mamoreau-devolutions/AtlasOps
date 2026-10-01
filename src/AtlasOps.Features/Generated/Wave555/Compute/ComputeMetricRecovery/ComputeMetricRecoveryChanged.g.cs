namespace AtlasOps.Features.Compute.ComputeMetricRecovery;

public sealed record ComputeMetricRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);