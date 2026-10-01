namespace AtlasOps.Features.Data.DataContractRecovery;

public sealed record DataContractRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);