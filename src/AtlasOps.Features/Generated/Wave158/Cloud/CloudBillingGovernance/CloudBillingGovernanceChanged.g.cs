namespace AtlasOps.Features.Cloud.CloudBillingGovernance;

public sealed record CloudBillingGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);