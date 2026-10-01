namespace AtlasOps.Features.Api.ApiHealthRecovery;

public sealed record ApiHealthRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);