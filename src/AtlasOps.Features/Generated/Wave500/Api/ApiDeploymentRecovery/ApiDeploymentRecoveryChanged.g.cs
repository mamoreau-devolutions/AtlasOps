namespace AtlasOps.Features.Api.ApiDeploymentRecovery;

public sealed record ApiDeploymentRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);