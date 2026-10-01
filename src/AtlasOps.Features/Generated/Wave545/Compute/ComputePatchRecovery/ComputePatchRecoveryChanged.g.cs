namespace AtlasOps.Features.Compute.ComputePatchRecovery;

public sealed record ComputePatchRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);