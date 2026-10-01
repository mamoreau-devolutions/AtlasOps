namespace AtlasOps.Features.Api.ApiEndpointRecovery;

public sealed record ApiEndpointRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);