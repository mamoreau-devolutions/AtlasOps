namespace AtlasOps.Features.ServiceManagement.ServiceDependencyRecovery;

public sealed record ServiceDependencyRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);