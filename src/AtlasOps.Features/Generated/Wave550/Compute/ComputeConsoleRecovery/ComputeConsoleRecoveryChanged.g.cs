namespace AtlasOps.Features.Compute.ComputeConsoleRecovery;

public sealed record ComputeConsoleRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);