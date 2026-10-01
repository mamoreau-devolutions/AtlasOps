namespace AtlasOps.Features.Edge.EdgeDeploymentRecovery;

public sealed record EdgeDeploymentRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);