namespace AtlasOps.Features.ServiceManagement.ServiceDependencyGovernance;

public sealed record ServiceDependencyGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);