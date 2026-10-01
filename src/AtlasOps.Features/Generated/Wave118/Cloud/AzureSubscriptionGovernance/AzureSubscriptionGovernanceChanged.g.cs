namespace AtlasOps.Features.Cloud.AzureSubscriptionGovernance;

public sealed record AzureSubscriptionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);