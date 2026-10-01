namespace AtlasOps.Features.Cloud.AwsAccountOptimization;

public sealed record AwsAccountOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);