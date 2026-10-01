namespace AtlasOps.Features.Cloud.AwsAccountGovernance;

public sealed record AwsAccountGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);