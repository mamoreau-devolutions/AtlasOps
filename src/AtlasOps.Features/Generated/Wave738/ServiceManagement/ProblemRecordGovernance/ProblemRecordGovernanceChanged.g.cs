namespace AtlasOps.Features.ServiceManagement.ProblemRecordGovernance;

public sealed record ProblemRecordGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);