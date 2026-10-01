namespace AtlasOps.Features.Cloud.CloudNetworkGovernance;

public sealed record CloudNetworkGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);