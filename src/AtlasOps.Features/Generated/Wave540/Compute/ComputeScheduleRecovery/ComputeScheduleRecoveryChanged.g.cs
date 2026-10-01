namespace AtlasOps.Features.Compute.ComputeScheduleRecovery;

public sealed record ComputeScheduleRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);