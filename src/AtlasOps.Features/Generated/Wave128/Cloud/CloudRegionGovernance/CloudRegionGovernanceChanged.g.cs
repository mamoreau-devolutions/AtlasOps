namespace AtlasOps.Features.Cloud.CloudRegionGovernance;

public sealed record CloudRegionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);