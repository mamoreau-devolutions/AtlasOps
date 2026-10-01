namespace AtlasOps.Features.Compute.ComputeImageRecovery;

public sealed record ComputeImageRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);