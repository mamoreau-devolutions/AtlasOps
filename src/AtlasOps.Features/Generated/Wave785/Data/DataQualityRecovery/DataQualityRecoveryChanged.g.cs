namespace AtlasOps.Features.Data.DataQualityRecovery;

public sealed record DataQualityRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);