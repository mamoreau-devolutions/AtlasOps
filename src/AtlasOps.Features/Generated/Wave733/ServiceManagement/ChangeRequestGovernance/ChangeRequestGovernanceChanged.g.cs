namespace AtlasOps.Features.ServiceManagement.ChangeRequestGovernance;

public sealed record ChangeRequestGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);