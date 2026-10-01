namespace AtlasOps.Features.Api.ApiTokenRecovery;

public sealed record ApiTokenRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);