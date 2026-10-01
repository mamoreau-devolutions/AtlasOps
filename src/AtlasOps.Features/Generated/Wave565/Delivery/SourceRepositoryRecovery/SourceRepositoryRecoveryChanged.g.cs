namespace AtlasOps.Features.Delivery.SourceRepositoryRecovery;

public sealed record SourceRepositoryRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);