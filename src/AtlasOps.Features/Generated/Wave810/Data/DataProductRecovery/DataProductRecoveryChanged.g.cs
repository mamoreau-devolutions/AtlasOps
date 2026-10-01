namespace AtlasOps.Features.Data.DataProductRecovery;

public sealed record DataProductRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);