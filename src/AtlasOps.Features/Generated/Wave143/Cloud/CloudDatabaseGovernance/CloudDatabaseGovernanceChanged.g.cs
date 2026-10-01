namespace AtlasOps.Features.Cloud.CloudDatabaseGovernance;

public sealed record CloudDatabaseGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);