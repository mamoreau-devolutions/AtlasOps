namespace AtlasOps.Features.Cloud.CloudBillingOptimization;

public sealed record CloudBillingOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);