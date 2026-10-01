namespace AtlasOps.Features.ServiceManagement.ServiceRequestGovernance;

public sealed record ServiceRequestGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);