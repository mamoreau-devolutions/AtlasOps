namespace AtlasOps.Features.Cloud.AzureSubscriptionOptimization;

public sealed record AzureSubscriptionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);