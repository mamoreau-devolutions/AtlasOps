namespace AtlasOps.Features.Compute.ComputeLifecycleRecovery;

public sealed record ComputeLifecycleRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);