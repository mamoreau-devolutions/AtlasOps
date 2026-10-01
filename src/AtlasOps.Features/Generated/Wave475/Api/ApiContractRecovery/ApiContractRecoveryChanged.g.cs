namespace AtlasOps.Features.Api.ApiContractRecovery;

public sealed record ApiContractRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);