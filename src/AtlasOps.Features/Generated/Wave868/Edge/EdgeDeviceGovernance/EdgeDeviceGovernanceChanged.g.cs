namespace AtlasOps.Features.Edge.EdgeDeviceGovernance;

public sealed record EdgeDeviceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);