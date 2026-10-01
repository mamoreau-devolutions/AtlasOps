namespace AtlasOps.Features.Compute.ComputeScaleSetRecovery;

public sealed record ComputeScaleSetRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);