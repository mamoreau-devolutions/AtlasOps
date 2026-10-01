namespace AtlasOps.Features.ServiceManagement.ServiceOwnerGovernance;

public sealed record ServiceOwnerGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);