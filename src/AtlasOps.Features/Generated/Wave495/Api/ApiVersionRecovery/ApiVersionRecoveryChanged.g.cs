namespace AtlasOps.Features.Api.ApiVersionRecovery;

public sealed record ApiVersionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);