namespace AtlasOps.Features.Api.ApiClientRecovery;

public sealed record ApiClientRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);