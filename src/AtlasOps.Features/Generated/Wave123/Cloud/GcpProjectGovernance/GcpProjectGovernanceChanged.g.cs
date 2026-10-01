namespace AtlasOps.Features.Cloud.GcpProjectGovernance;

public sealed record GcpProjectGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);