namespace AtlasOps.Features.Data.DataTransformRecovery;

public sealed record DataTransformRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);