namespace AtlasOps.Features.Cloud.CloudFunctionGovernance;

public sealed record CloudFunctionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);